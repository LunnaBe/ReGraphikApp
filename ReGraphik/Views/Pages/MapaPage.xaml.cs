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

        private void MapaBrowser_Navigated(object sender,
            System.Windows.Navigation.NavigationEventArgs e)
        {
            _mapaCarregado = true;
            MapaPlaceholder.Visibility = Visibility.Collapsed;
        }

        private async Task<List<PontosColeta>> BuscarPontosAsync(string cidade)
        {
            var lista = new List<PontosColeta>();
            try
            {
                // Substitua pela URL real da sua API
                var url = $"https://webregraphik.runasp.net/api/pontoscoleta";

                // Para fins de teste, vamos usar dados estáticos simulados
                var json = await _http.GetStringAsync(url);

                // Configurações para desserialização, ignorando diferenças de maiúsculas/minúsculas
                var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var resultado = JsonSerializer.Deserialize<List<PontosColeta>>(json, opcoes);

                if (resultado != null)
                {
                    // Filtra os pontos pela cidade, ignorando maiúsculas/minúsculas
                    lista = resultado
                        .Where(p => p.Cidade.Contains(cidade, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    for (int i = 0; i < lista.Count; i++)
                    {
                        // Para cada ponto, tente obter as coordenadas geográficas usando a API do Google Geocoding
                        _latLngs[i] = (-23.5505, -46.6333);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log do erro para depuração
                System.Diagnostics.Debug.WriteLine("Erro ao buscar na API ReGraphik: " + ex.Message);
                MessageBox.Show($"Erro ao carregar pontos da nuvem: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                marcadoresJs.Append($@"{{
                    ""idx"": {i},
                    ""nome"": ""{EscJs(p.NomePonto)}"",
                    ""endereco"": ""{EscJs(p.Cidade)}"",
                    ""tipos"": ""{EscJs(p.ResiduosAceitos)}"",
                    ""lat"": {ll.lat.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                    ""lng"": {ll.lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}
                }}");
            }
            marcadoresJs.Append("]");

            double centerLat = -23.5505, centerLng = -46.6333;
            int zoom = 12;
            if (_latLngs.Count > 0)
            {
                centerLat = _latLngs.Values.Average(ll => ll.lat);
                centerLng = _latLngs.Values.Average(ll => ll.lng);
                zoom = pontos.Count == 1 ? 15 : 13;
            }

            return $@"<!DOCTYPE html>
<html>
<head>
<meta charset=""utf-8"">
<link rel=""stylesheet"" href=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.css""/>
<style>
  * {{ margin:0; padding:0; box-sizing:border-box; }}
  body {{ font-family:'Segoe UI',sans-serif; }}
  #map {{ width:100%; height:100vh; }}
  .popup-header {{ background:#2563EB; color:white; padding:10px 14px;
                   margin:-14px -14px 10px -14px; border-radius:4px 4px 0 0;
                   font-weight:600; font-size:13px; }}
  .popup-row {{ font-size:12px; color:#475569; margin-bottom:6px; }}
  .popup-row span {{ color:#1E293B; font-weight:500; }}
  .badge {{ display:inline-block; background:#D1FAE5; color:#065F46;
            padding:2px 8px; border-radius:10px; font-size:11px;
            font-weight:600; margin-top:4px; }}
  .leaflet-popup-content-wrapper {{ border-radius:10px;
    box-shadow:0 4px 20px rgba(0,0,0,0.15); }}
</style>
</head>
<body>
<div id=""map""></div>
<script src=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.js""></script>
<script>
var pontos = {marcadoresJs};
var map = L.map('map', {{
    center: [{centerLat.ToString(System.Globalization.CultureInfo.InvariantCulture)},
             {centerLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}],
    zoom: {zoom}
}});
L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
    attribution: '© OpenStreetMap', maxZoom: 19
}}).addTo(map);
var iconPonto = L.divIcon({{
    html: '<div style=""background:#2563EB;width:30px;height:30px;border-radius:50% 50% 50% 0;transform:rotate(-45deg);border:3px solid white;box-shadow:0 2px 8px rgba(0,0,0,0.3)""></div>',
    iconSize: [30,30], iconAnchor: [15,30], popupAnchor: [0,-30], className: ''
}});
var markers = [];
pontos.forEach(function(p) {{
    if (p.lat === 0 && p.lng === 0) return;
    var m = L.marker([p.lat, p.lng], {{ icon: iconPonto }}).addTo(map);
    m.bindPopup(
        '<div class=""popup-header"">' + p.nome + '</div>' +
        '<div class=""popup-row"">📍 <span>' + p.endereco + '</span></div>' +
        '<div class=""badge"">♻ ' + p.tipos + '</div>'
    , {{ maxWidth: 260 }});
    markers[p.idx] = m;
}});
var validos = markers.filter(Boolean);
if (validos.length > 0) {{
    map.fitBounds(L.featureGroup(validos).getBounds().pad(0.2));
}}
window.centralizarPonto = function(idx) {{
    var m = markers[idx];
    if (m) {{ map.setView(m.getLatLng(), 16); m.openPopup(); }}
}};
</script>
</body>
</html>";
        }

        private static string EscJs(string s) =>
            s.Replace("\\", "\\\\").Replace("\"", "\\\"")
             .Replace("\n", " ").Replace("\r", "");
    }
}