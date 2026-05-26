using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ApiRestReGraphik.Models;

namespace ReGraphik.Views.Pages
{
    public partial class MapaPage : Page
    {
        private readonly HttpClient _http = new();
        private const string ApiKey = "AIzaSyCPeDl0hmzFeROHcUxPbnQUvAhOA_N-ros";
        private bool _mapaCarregado = false;
        private List<PontosColeta> _pontosAtuais = new();
        private readonly Dictionary<int, (double lat, double lng)> _latLngs = new();

        public MapaPage()
        {
            InitializeComponent();
        }

        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            var cidade = TxtCidade.Text.Trim();

            if (string.IsNullOrWhiteSpace(cidade))
            {
                MessageBox.Show("Por favor, digite o nome de uma cidade.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 1. Envia a requisição POST para a API fazer a sincronização direto no Firebase
                string urlSincronizar = $"https://webregraphik.runasp.net/api/PontosColeta/sincronizar?cidade={Uri.EscapeDataString(cidade)}";
                var conteudoVazio = new StringContent("", System.Text.Encoding.UTF8, "application/json");

                var response = await _http.PostAsync(urlSincronizar, conteudoVazio);

                if (response.IsSuccessStatusCode)
                {
                    // 2. Se a sincronização deu certo, chamamos a função para trazer os dados atualizados para a tela
                    MessageBox.Show($"Sincronização de '{cidade}' concluída com sucesso com os dados do Google!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Busca os pontos atualizados para renderizar no mapa do Leaflet
                    var pontosSalvos = await BuscarPontosDoBancoAsync();

                    // Filtra na tela para exibir apenas os pontos da cidade buscada
                    _pontosAtuais = pontosSalvos
                        .Where(p => p.Cidade != null && p.Cidade.Contains(cidade, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    // Atualiza o dicionário de coordenadas da tela
                    _latLngs.Clear();
                    for (int i = 0; i < _pontosAtuais.Count; i++)
                    {
                        _latLngs[i] = (_pontosAtuais[i].Lat, _pontosAtuais[i].Lng);
                    }

                    // Atualiza a ListView lateral com os pontos encontrados
                    ListaPontos.ItemsSource = null;
                    ListaPontos.ItemsSource = _pontosAtuais;

                    // Esconde o placeholder do mapa enquanto carrega os dados
                    EstadoVazio.Visibility = Visibility.Collapsed;

                    // Alimenta o componente visual do mapa
                    CarregarMapa(_pontosAtuais);
                }
                else
                {
                    string erroApi = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro na API ao sincronizar: {erroApi}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao se conectar com o servidor: {ex.Message}", "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PontoItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is PontosColeta ponto)
            {
                if (_mapaCarregado)
                {
                    try
                    {
                        var idx = _pontosAtuais.IndexOf(ponto);
                        MapaBrowser.InvokeScript("centralizarPonto", idx);
                    }
                    catch { }
                }
            }
        }

        private void MapaBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            _mapaCarregado = true;
            MapaPlaceholder.Visibility = Visibility.Collapsed;
        }

        private async Task<List<PontosColeta>> BuscarPontosDoBancoAsync()
        {
            var lista = new List<PontosColeta>();
            try
            {
                // Rota GET pura da API que lista tudo que está gravado no Firebase
                var urlApi = "https://webregraphik.runasp.net/api/PontosColeta";
                var resposta = await _http.GetAsync(urlApi);

                if (resposta.IsSuccessStatusCode)
                {
                    var json = await resposta.Content.ReadAsStringAsync();
                    var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var pontosRetornados = JsonSerializer.Deserialize<List<PontosColeta>>(json, opcoes);

                    if (pontosRetornados != null)
                    {
                        lista = pontosRetornados;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao listar pontos do banco: " + ex.Message);
            }
            return lista;
        }

        private void CarregarMapa(List<PontosColeta> pontos)
        {
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

                // Trata coordenadas zeradas para evitar que o mapa quebre
                double latitude = ll.lat != 0 ? ll.lat : -23.55052; // Fallback SP
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
        var map = L.map('map').setView([-15.7801, -47.9292], 4); // Visão inicial Brasil
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