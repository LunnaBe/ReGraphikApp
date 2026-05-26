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
            if (string.IsNullOrWhiteSpace(cidade)) return;

            EstadoVazio.Visibility = Visibility.Collapsed;
            EstadoCarregando.Visibility = Visibility.Visible;
            ListaPontos.ItemsSource = null;
            _latLngs.Clear();

            var pontos = await BuscarPontosAsync(cidade);

            _pontosAtuais = pontos;
            EstadoCarregando.Visibility = Visibility.Collapsed;
            TxtContagem.Text = pontos.Count.ToString();
            ListaPontos.ItemsSource = pontos;

            if (pontos.Count == 0)
                EstadoVazio.Visibility = Visibility.Visible;

            CarregarMapa(pontos);
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

        private async Task<List<PontosColeta>> BuscarPontosAsync(string cidade)
        {
            var lista = new List<PontosColeta>();
            try
            {
                // Monta a URL da API do backend, garantindo que o nome da cidade seja corretamente codificado para a URL
                var urlApi = $"https://webregraphik.runasp.net/api/PontosColeta/google?cidade={Uri.EscapeDataString(cidade)}";

                // Consulta a API do backend para obter os pontos de coleta da cidade
                var resposta = await _http.GetAsync(urlApi);

                // Se a resposta for bem-sucedida, processa os dados retornados
                if (resposta.IsSuccessStatusCode)
                {
                    var json = await resposta.Content.ReadAsStringAsync();
                    var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var pontosRetornados = JsonSerializer.Deserialize<List<PontosColeta>>(json, opcoes);

                    if (pontosRetornados != null)
                    {
                        lista = pontosRetornados;
                    }

                    _latLngs.Clear();
                    for (int i = 0; i < lista.Count; i++)
                    {
                        _latLngs[i] = (lista[i].Lat, lista[i].Lng);
                    }
                }
                else
                {
                    // Se o backend retornou erro (Cidade não cadastrada), lê o texto enviado pela API
                    var mensagemErroDoServidor = await resposta.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        mensagemErroDoServidor,
                        "Busca Não Permitida",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao consultar API Backend: " + ex.Message);
                MessageBox.Show(
                    $"Não foi possível processar a requisição.\n\nDetalhes: {ex.Message}",
                    "Erro de Comunicação",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
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