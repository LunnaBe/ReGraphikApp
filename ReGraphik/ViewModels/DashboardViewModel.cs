using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ReGraphik.Models; 
using ApiRestReGraphik.Models;
using ReGraphik.Models; // Certifique-se de que a classe Residuo está acessível no WPF

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
            // Substitua pela URL da sua API rodando localmente (ex: porta 5001, 7200, etc)
            _httpClient.BaseAddress = new Uri("https://localhost:7001/api/");

            Task.Run(async () => await CarregarDadosDaApiAsync());
        }

        /// Método para buscar da API e fazer cálculos
        
        private async Task CarregarDadosDaApiAsync()
        {
            try
            {
                /// Faz a requisição GET para o endpoint de residuos da API
                HttpResponseMessage response = await _httpClient.GetAsync("Residuo");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();

                    /// Converte o JSON vindo da API para uma lista C#
                    var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    List<Residuo> todosResiduos = JsonSerializer.Deserialize<List<Residuo>>(jsonResult, opcoes);

                    if (todosResiduos != null && todosResiduos.Any())
                    {
                        /// Atualiza as propriedades na Thread da UI
                        App.Current.Dispatcher.Invoke(() =>
                        {
                           
                            ResiduosDb = todosResiduos.Count;

                            // Quantidade de reaproveitados (Ajuste a string "Reaproveitado" conforme salvo no Firebase)
                            Reaproveitar = todosResiduos.Count(r => r.Status == "Reaproveitado");

                            // Quantidade em estoque (Ajuste a string "Em Estoque" conforme salvo no Firebase)
                            Estoque = todosResiduos.Count(r => r.Status == "Em Estoque");

                            // Cálculo do Total Econômico
                            // Como o modelo não possui valor financeiro, estou multiplicando a Quantidade por um valor fictício (ex: R$ 5,50/kg)
                            double valorCalculado = todosResiduos.Sum(r => r.Quantidade * 5.50);
                            Total = valorCalculado.ToString("C2"); // Formata para R$

                            /// Pega os últimos 5 resíduos ordenados pela data mais recente
                            var ultimos = todosResiduos.OrderByDescending(r => r.DataCadastro).Take(5);
                            UltimosResiduos = new ObservableCollection<Residuo>(ultimos);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar dados: {ex.Message}");
            }
        }
    }
}