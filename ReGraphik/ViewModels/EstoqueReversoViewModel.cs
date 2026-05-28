using ApiRestReGraphik.Models;
using ReGraphik.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ReGraphik.Models;

namespace ReGraphik.ViewModels
{
    public class EstoqueReversoViewModel : BaseViewModel
    {
        // Instancia única do HttpClient para toda a classe, seguindo as melhores práticas
        private readonly HttpClient _http = new();

        // Propriedade para armazenar a lista de resíduos, que pode ser vinculada à interface para exibição
        public ObservableCollection<Residuo> ListaResiduos { get; set; } = new ObservableCollection<Residuo>();

        // Comando para acionar a função de sugestão, que pode ser vinculada a um botão na interface
        public ICommand SugestaoCommand { get; set; }



        public EstoqueReversoViewModel()
        {
            // Inicializa o comando de sugestão com a função que será executada ao clicar no botão.
            SugestaoCommand = new RelayCommand(TelaSugestao);

            // Carrega os dados do estoque reverso do banco assim que a ViewModel for criada . 
            _ = CarregarEstoqueDoBancoAsync();
        }

        private async Task CarregarEstoqueDoBancoAsync()
        {
            try
            {
                // URL da API para buscar os resíduos do estoque reverso
                string urlApi = "https://webregraphik.runasp.net/api/Residuo";

                var resposta = await _http.GetAsync(urlApi);

                // Verifica se a resposta foi bem-sucedida antes de tentar ler o conteúdo
                if (resposta.IsSuccessStatusCode)
                {
                    var json = await resposta.Content.ReadAsStringAsync();
                    var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    // Deserializa usando a sua classe de model real 'Residuo'
                    var listaResiduos = JsonSerializer.Deserialize<List<Residuo>>(json, opcoes);

                    // Atualiza a coleção de resíduos na interface, garantindo que seja feito no thread correto
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ListaResiduos.Clear();
                        if (listaResiduos != null)
                        {
                            foreach (var residuo in listaResiduos)
                            {
                                ListaResiduos.Add(residuo);
                            }
                        }
                    });
                }
                else
                {
                    MessageBox.Show("Não foi possível buscar a lista de resíduos no servidor.",
                                    "Erro de Comunicação", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao conectar na API de Estoque: " + ex.Message);
            }
        }

        private void TelaSugestao()
        {
            // Lógica para buscar sugestões com base no resíduo selecionado.
            MessageBox.Show($"Buscando sugestões para o item ID: ", "Sugestões");
        }
    }
}
