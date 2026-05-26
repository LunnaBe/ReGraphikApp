using ApiRestReGraphik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;

namespace ReGraphik.Views.Pages
{
    /// <summary>
    /// Interaction logic for EstoqueReversoPage.xaml
    /// </summary>
    public partial class EstoqueReversoPage : Page
    {
        // Instancia única do HttpClient para toda a classe, seguindo as melhores práticas
        private readonly HttpClient _http = new();
        public EstoqueReversoPage()
        {
            InitializeComponent();

            // Carrega o estoque do banco assim que a página for carregada
            Loaded += async (s, e) => await CarregarEstoqueDoBancoAsync();
        }

        private async Task CarregarEstoqueDoBancoAsync()
        {
            try
            {
                // URL do endpoint de resíduos da sua API
                string urlApi = "https://webregraphik.runasp.net/api/Residuos";

                var resposta = await _http.GetAsync(urlApi);

                // Verifica se a resposta foi bem-sucedida antes de tentar ler o conteúdo
                if (resposta.IsSuccessStatusCode)
                {
                    var json = await resposta.Content.ReadAsStringAsync();
                    var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    // Deserializa usando a sua classe de model real 'Residuo'
                    var listaResiduos = JsonSerializer.Deserialize<List<Residuo>>(json, opcoes);

                    // Vincula a lista do Firebase direto na DataGrid do WPF
                    DgEstoque.ItemsSource = listaResiduos;
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

        private void BtnSugestoes_Click(object sender, RoutedEventArgs e)
        {
            var botao = sender as Button;
            // Converte o contexto da linha selecionada para a sua model real de Residuo
            var residuoSelecionado = botao?.DataContext as Residuo;

            if (residuoSelecionado != null)
            {
                // Dispara o alerta usando a sua propriedade real TipoResiduo
                MessageBox.Show($"Buscando soluções ecológicas e ideias de reuso para: {residuoSelecionado.TipoResiduo}",
                                "Sugestões de Reuso Inteligente", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
