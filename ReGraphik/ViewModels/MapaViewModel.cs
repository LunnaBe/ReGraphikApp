using ReGraphik.Commands;
using ReGraphik.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReGraphik.ViewModels
{
    public class MapaViewModel : BaseViewModel
    {
        // Instancia única do HttpClient para toda a classe, seguindo as melhores práticas
        private readonly HttpClient _http = new();
        private bool _mapaCarregado = false;
        private string _cidade = string.Empty;
        private ObservableCollection<PontosColeta> _pontosAtuais = new();
        private bool _isCarregando;

        public event Action<int>? SolicitouFocoNoMapa;

        public event Action<string>? SolicitouNavegacaoMapa;

        // Propriedades públicas para vinculação à interface
        public string Cidade
        {
            get => _cidade;
            set { _cidade = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PontosColeta> PontosAtuais
        {
            get => _pontosAtuais;
            set { _pontosAtuais = value; OnPropertyChanged(); OnPropertyChanged(nameof(MostrarEstadoVazio)); }
        }

        // Referências públicas para os elementos visuais, a serem atribuídas pela View
        public bool IsCarregando
        {
            get => _isCarregando;
            set
            {
                _isCarregando = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MostrarEstadoVazio));
            }
        }

        // Referências públicas para os elementos visuais, a serem atribuídas pela View
        public bool MostrarEstadoVazio => !IsCarregando && (PontosAtuais == null || PontosAtuais.Count == 0);

        // Comando para iniciar a busca dos pontos de coleta
        public ICommand BuscarCommand { get; set; }

        public MapaViewModel()
        {
            // Inicializa o comando de busca com a função assíncrona correspondente
            BuscarCommand = new RelayCommand(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await BuscarAsync();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Erro fatal interceptado: " + ex.Message);
                    }
                });
            });
        }

        private async Task BuscarAsync()
        {
            if (string.IsNullOrWhiteSpace(Cidade))
            {
                MessageBox.Show("Por favor, digite o nome de uma cidade.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsCarregando = true;

            try
            {
                // Faz uma requisição POST para a API para sincronizar os pontos de coleta da cidade informada
                string urlSincronizar = $"https://webregraphik.runasp.net/api/PontosColeta/sincronizar?cidade={Uri.EscapeDataString(Cidade)}";
                var conteudoVazio = new StringContent("", Encoding.UTF8, "application/json");

                var response = await _http.PostAsync(urlSincronizar, conteudoVazio);

                if (response.IsSuccessStatusCode)
                {
                    var pontosSalvos = await BuscarPontosDoBancoAsync();

                    var filtrados = pontosSalvos
                        .Where(p => p.Cidade != null && p.Cidade.Contains(Cidade, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    PontosAtuais = new ObservableCollection<PontosColeta>(filtrados);

                    // Após atualizar a coleção, recarrega o mapa para refletir os novos pontos
                    CarregarMapa(filtrados);
                    IsCarregando = false;
                }
                else
                {
                    // Em caso de erro na API, exibe a mensagem de erro retornada
                    IsCarregando = false;
                    string erroApi = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro na API ao sincronizar: {erroApi}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                IsCarregando = false;
                MessageBox.Show($"Erro ao se conectar com o servidor: {ex.Message}", "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Método público para ser chamado pela View quando um ponto de coleta for selecionado na lista, solicitando o foco no mapa
        public void FocarNoPonto(PontosColeta ponto)
        {
            if (_mapaCarregado && PontosAtuais != null)
            {
                // Encontra o índice do ponto selecionado na coleção atual
                var idx = PontosAtuais.IndexOf(ponto);
                if (idx >= 0)
                {
                    // Solicita à View que centralize o ponto correspondente no mapa
                    SolicitouFocoNoMapa?.Invoke(idx);
                }
            }
        }

        // Método público para ser chamado pela View quando o mapa estiver completamente carregado
        public void NotificarMapaCarregado()
        {
            _mapaCarregado = true;
        }

        private async Task<List<PontosColeta>> BuscarPontosDoBancoAsync()
        {
            var lista = new List<PontosColeta>();
            try
            {
                // Faz uma requisição GET para a API para obter os pontos de coleta
                var urlApi = "https://webregraphik.runasp.net/api/PontosColeta";
                var resposta = await _http.GetAsync(urlApi);

                if (resposta.IsSuccessStatusCode)
                {
                    // Lê o conteúdo JSON da resposta e desserializa para a lista de pontos de coleta
                    var json = await resposta.Content.ReadAsStringAsync();
                    var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<PontosColeta>>(json, opcoes) ?? lista;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao listar pontos: " + ex.Message);
            }
            return lista;
        }

        private void CarregarMapa(List<PontosColeta> pontos)
        {
            // Gera o HTML do mapa com os pontos de coleta e salva em um arquivo temporário
            var html = GerarHtml(pontos);
            var tmpFile = Path.Combine(Path.GetTempPath(), "regraphik_mapa.html");
            File.WriteAllText(tmpFile, html, Encoding.UTF8);
            _mapaCarregado = false;

            // Solicita à View que navegue para o arquivo HTML gerado
            SolicitouNavegacaoMapa?.Invoke(tmpFile);
        }

        // Método público para ser chamado pela View quando um ponto de coleta for selecionado na lista
        private string GerarHtml(List<PontosColeta> pontos)
        {
            var marcadoresJs = new StringBuilder("[");
            for (int i = 0; i < pontos.Count; i++)
            {
                var p = pontos[i];
                if (i > 0) marcadoresJs.Append(",");

                // Se as coordenadas forem zero, utiliza uma localização padrão (São Paulo) para evitar erros no mapa
                string latitude = p.Lat != 0 ? p.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture) : "-23.55052";
                string longitude = p.Lng != 0 ? p.Lng.ToString(System.Globalization.CultureInfo.InvariantCulture) : "-46.633308";

                marcadoresJs.Append($@"{{
                    ""idx"": {i},
                    ""nome"": ""{EscJs(p.NomePonto)}"",
                    ""endereco"": ""{EscJs(p.Cidade)}"",
                    ""tipos"": ""{EscJs(p.ResiduosAceitos)}"",
                    ""lat"": {latitude},
                    ""lng"": {longitude}
                }}");
            }
            marcadoresJs.Append("]");

            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <meta http-equiv='X-UA-Compatible' content='IE=edge' />
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' />
    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
    <style>
        html, body, #map {{ height: 100%; margin: 0; padding: 0; font-family: sans-serif; }}
        .popup-custom {{ font-size: 14px; line-height: 1.4; }}
        .popup-title {{ font-weight: bold; color: #2e7d32; margin-bottom: 4px; }}
    </style>
</head>
<body>
    <div id='map'></div>
    <script>
        var map = L.map('map').setView([-15.7801, -47.9292], 4);
        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
            attribution: '&copy; OpenStreetMap contributors'
        }}).addTo(map);

        var pontos = {marcadoresJs.ToString()};
        var marcadores = [];

        if (pontos.length > 0) {{
            var bounds = [];
            pontos.forEach(function(p) {{
                var marker = L.marker([p.lat, p.lng]).addTo(map);
                var conteudo = '<div class=""popup-custom"">' +
                               '<div class=""popup-title"">' + p.nome + '</div>' +
                               '<div><b>Endereço:</b> ' + p.endereco + '</div>' +
                               '<div><b>Resíduos:</b> ' + p.tipos + '</div>' +
                               '</div>';
                marker.bindPopup(conteudo);
                marcadores.push(marker);
                bounds.push([p.lat, p.lng]);
            }});
            map.fitBounds(bounds);
        }}

        function centralizarPonto(index) {{
            if (marcadores[index]) {{
                var m = marcadores[index];
                map.setView(m.getLatLng(), 16);
                m.openPopup();
            }}
        }}
    </script>
</body>
</html>";
        }

        // Método público para ser chamado pela View quando um ponto de coleta for selecionado na lista, solicitando o foco no mapa
        private string EscJs(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return "";
            return texto.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", " ");
        }
    }
}