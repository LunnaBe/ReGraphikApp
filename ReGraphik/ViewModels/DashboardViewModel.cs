using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ReGraphik.Models; 
using ApiRestReGraphik.Models;
using ReGraphik.Models;
using LiveCharts;
using LiveCharts.Wpf;

namespace ReGraphik.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        ///  Propriedades para os Cards
        /// </summary>
        private int _residuosDb;
        public int ResiduosDb
        {
            get => _residuosDb;
            set { _residuosDb = value; OnPropertyChanged(); }
        }

        private int _reaproveitar;
        public int Reaproveitar
        {
            get => _reaproveitar;
            set { _reaproveitar = value; OnPropertyChanged(); }
        }

        private int _estoque;
        public int Estoque
        {
            get => _estoque;
            set { _estoque = value; OnPropertyChanged(); }
        }

        private string _total; /// coloquei string para formatar como Moeda 
        public string Total
        {
            get => _total;
            set { _total = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Propriedade para a Tabela 
        /// </summary>
        private ObservableCollection<Residuo> _ultimosResiduos;
        public ObservableCollection<Residuo> UltimosResiduos
        {
            get => _ultimosResiduos;
            set { _ultimosResiduos = value; OnPropertyChanged(); }
        }

        /// Construtor
        public DashboardViewModel()
        {
            _httpClient = new HttpClient();

            // AJUSTE FEITO AQUI: URL aponta apenas para a raiz da API
            _httpClient.BaseAddress = new Uri("https://webregraphik.runasp.net/api/");

            Task.Run(async () => await CarregarDadosDaApiAsync());
        }

        /// Método para buscar da API e fazer cálculos
        private async Task CarregarDadosDaApiAsync()
        {
            try
            {
                // Faz a requisição GET para o endpoint de Residuos
                // Como o BaseAddress é ".../api/", ele vai chamar ".../api/Residuo"
                HttpResponseMessage response = await _httpClient.GetAsync("Residuo");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();

                    var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    List<Residuo> todosResiduos = JsonSerializer.Deserialize<List<Residuo>>(jsonResult, opcoes);

                    if (todosResiduos != null && todosResiduos.Any())
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            ResiduosDb = todosResiduos.Count;
                            Reaproveitar = todosResiduos.Count(r => r.Status == "Reaproveitado");
                            Estoque = todosResiduos.Count(r => r.Status == "Em Estoque");

                            double valorCalculado = todosResiduos.Sum(r => r.Quantidade * 5.50);
                            Total = valorCalculado.ToString("C2");

                            var ultimos = todosResiduos.OrderByDescending(r => r.DataCadastro).Take(5);
                            UltimosResiduos = new ObservableCollection<Residuo>(ultimos);
                        });
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("A API conectou, mas a lista de resíduos veio vazia.", "Aviso");
                    }
                }
                else
                {
                    // Se a API retornar erro (ex: 404 Not Found, 500 Internal Server Error)
                    System.Windows.MessageBox.Show($"A API retornou um erro: {response.StatusCode}", "Erro HTTP");
                }
            }
            catch (Exception ex)
            {
                // Se o WPF não conseguir nem achar a API (ex: porta errada, API desligada)
                System.Windows.MessageBox.Show($"Falha de conexão: {ex.Message}", "Erro de Código");
            }
        }
    }
}