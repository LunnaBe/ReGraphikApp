using ApiRestReGraphik.Models;
using ReGraphik.Commands;
using System;
using System.Collections.Generic;
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
        private List<PontosColeta> _pontosAtuais = new();
        private readonly Dictionary<int, (double lat, double lng)> _latLngs = new();

        // Propriedades públicas para vinculação à interface
        public string Cidade
        {
            get => _cidade;
            set { _cidade = value; OnPropertyChanged(); }
        }

        public List<PontosColeta> PontosAtuais
        {
            get => _pontosAtuais;
            set { _pontosAtuais = value; OnPropertyChanged(); }
        }

        // Referências para elementos visuais (definidos na View)
        public WebBrowser MapaBrowser { get; set; }
        public ListView ListaPontos { get; set; }
        public UIElement EstadoVazio { get; set; }
        public UIElement MapaPlaceholder { get; set; }
        public UIElement EstadoCarregando { get; set; }

        public ICommand BuscarCommand { get; set; }

        public MapaViewModel()
        {
            // Inicializa o comando de busca com a função assíncrona correspondente
            BuscarCommand = new RelayCommand(async () => await BuscarAsync());

            // Inicializa as referências dos elementos visuais como nulos, para serem atribuídas posteriormente pela View
            MapaBrowser = new WebBrowser();
            ListaPontos = new ListView();
            EstadoVazio = new UIElement();
            MapaPlaceholder = new UIElement();
            EstadoCarregando = new UIElement();
        }

        private async Task BuscarAsync()
        {
            if (string.IsNullOrWhiteSpace(Cidade))
            {
                MessageBox.Show("Por favor, digite o nome de uma cidade.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DefinirEstadoVisual(carregando: true);

            try
            {
                string urlSincronizar = $"https://webregraphik.runasp.net/api/PontosColeta/sincronizar?cidade={Uri.EscapeDataString(Cidade)}";
                var conteudoVazio = new StringContent("", Encoding.UTF8, "application/json");

                var response = await _http.PostAsync(urlSincronizar, conteudoVazio);

                if (response.IsSuccessStatusCode)
                {
                    var pontosSalvos = await BuscarPontosDoBancoAsync();

                    // Filtra e atualiza a propriedade observável
                    PontosAtuais = pontosSalvos
                        .Where(p => p.Cidade != null && p.Cidade.Contains(Cidade, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    _latLngs.Clear();
                    for (int i = 0; i < PontosAtuais.Count; i++)
                    {
                        _latLngs[i] = (PontosAtuais[i].Lat, PontosAtuais[i].Lng);
                    }

                    if (ListaPontos != null)
                    {
                        ListaPontos.ItemsSource = PontosAtuais;
                    }

                    DefinirEstadoVisual(carregando: false);
                    CarregarMapa(PontosAtuais);
                }
                else
                {
                    DefinirEstadoVisual(carregando: false);
                    string erroApi = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro na API ao sincronizar: {erroApi}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                DefinirEstadoVisual(carregando: false);
                MessageBox.Show($"Erro ao se conectar com o servidor: {ex.Message}", "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void FocarNoPonto(PontosColeta ponto)
        {
            if (_mapaCarregado && MapaBrowser != null)
            {
                try
                {
                    var idx = PontosAtuais.IndexOf(ponto);
                    if (idx >= 0)
                    {
                        MapaBrowser.InvokeScript("centralizarPonto", idx);
                    }
                }
                catch { }
            }
        }

        public void NotificarMapaCarregado()
        {
            _mapaCarregado = true;
            if (MapaPlaceholder != null)
                MapaPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void DefinirEstadoVisual(bool carregando)
        {
            if (EstadoCarregando != null) EstadoCarregando.Visibility = carregando ? Visibility.Visible : Visibility.Collapsed;
            if (EstadoVazio != null) EstadoVazio.Visibility = (!carregando && PontosAtuais.Count == 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        private async Task<List<PontosColeta>> BuscarPontosDoBancoAsync()
        {
            var lista = new List<PontosColeta>();
            try
            {
                var urlApi = "https://webregraphik.runasp.net/api/PontosColeta";
                var resposta = await _http.GetAsync(urlApi);

                if (resposta.IsSuccessStatusCode)
                {
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
            if (MapaBrowser == null) return;

            var html = GerarHtml(pontos);
            var tmpFile = Path.Combine(Path.GetTempPath(), "regraphik_mapa.html");
            File.WriteAllText(tmpFile, html, Encoding.UTF8);
            _mapaCarregado = false;
            MapaBrowser.Navigate(new Uri(tmpFile));
        }

        private string GerarHtml(List<PontosColeta> pontos)
        {
            var marcadoresJs = new StringBuilder("[");
            for (int i = 0; i < pontos.Count; i++)
            {
                var p = pontos[i];
                _latLngs.TryGetValue(i, out var ll);
                if (i > 0) marcadoresJs.Append(",");

                double latitude = ll.lat != 0 ? ll.lat : -23.55052;
                double longitude = ll.lng != 0 ? ll.lng : -46.633308;

                marcadoresJs.Append($@"{{
                    ""idx"": {i},
                    ""nome"": ""{EscJs(p.NomePonto)}"",
                    ""endereco"": ""{EscJs(p.Cidade)}"",
                    ""tipos"": ""{EscJs(p.ResiduosAceitos)}"",
                    ""lat"": {latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                    ""lng"": {longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}
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

        private string EscJs(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return "";
            return texto.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", " ");
        }
    }
}