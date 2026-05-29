using ReGraphik.Commands;
using ReGraphik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ReGraphik.ViewModels
{
    public class ResiduoViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient;

        // Propriedades vinculadas aos campos da tela
        private string _tipoMaterial;
        public string TipoMaterial
        {
            get => _tipoMaterial;
            set { _tipoMaterial = value; OnPropertyChanged(); }
        }

        private string _especificacao;
        public string Especificacao
        {
            get => _especificacao;
            set { _especificacao = value; OnPropertyChanged(); }
        }

        private string _origem;
        public string Origem
        {
            get => _origem;
            set { _origem = value; OnPropertyChanged(); }
        }

        private string _projetoOrigem;
        public string ProjetoOrigem
        {
            get => _projetoOrigem;
            set { _projetoOrigem = value; OnPropertyChanged(); }
        }

        private string _quantidade;
        public string Quantidade
        {
            get => _quantidade;
            set { _quantidade = value; OnPropertyChanged(); }
        }

        private DateTime _data = DateTime.Now;
        public DateTime Data
        {
            get => _data;
            set { _data = value; OnPropertyChanged(); }
        }

        private string _condicao;
        public string Condicao
        {
            get => _condicao;
            set { _condicao = value; OnPropertyChanged(); }
        }

        private string _comprimento;
        public string Comprimento
        {
            get => _comprimento;
            set { _comprimento = value; OnPropertyChanged(); }
        }

        private string _largura;
        public string Largura
        {
            get => _largura;
            set { _largura = value; OnPropertyChanged(); }
        }

        private string _observacoes;
        public string Observacoes
        {
            get => _observacoes;
            set { _observacoes = value; OnPropertyChanged(); }
        }

        private string _nomeArquivo = "Nenhum arquivo selecionado";
        public string NomeArquivo
        {
            get => _nomeArquivo;
            set { _nomeArquivo = value; OnPropertyChanged(); }
        }

        // Comandos para as ações da tela
        public ICommand SalvarResiduoCommand { get; }
        public ICommand LimparCommand { get; }

        public ICommand SelecionarArquivoCommand {  get; }

        // Construtor do ViewModel, onde é inicializado o HttpClient e os comandos
        public ResiduoViewModel()
        {
            // Configura o HttpClient com a URL base da API
            _httpClient = new HttpClient { BaseAddress = new Uri("https://webregraphik.runasp.net/") };

            SalvarResiduoCommand = new RelayCommand(async () => await SalvarResiduoAsync());
            SelecionarArquivoCommand = new RelayCommand(SelecionarArquivo);
            LimparCommand = new RelayCommand(LimparCampos);
        }

        private async Task SalvarResiduoAsync()
        {
            try
            {
                // Validações básicas antes do envio
                if (string.IsNullOrEmpty(TipoMaterial) || string.IsNullOrEmpty(Especificacao))
                {
                    MessageBox.Show("Por favor, preencha os campos obrigatórios (Tipo e Especificação).", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Cria o objeto Residuo com os dados do formulário
                var novoResiduo = new Residuo
                {
                    Id = Guid.NewGuid().ToString(), // Gera um ID único para o resíduo
                    TipoResiduo = TipoMaterial,
                    Especificacao = Especificacao,
                    Origem = Origem,
                    Projeto = ProjetoOrigem,
                    Quantidade = double.TryParse(Quantidade, out var quant) ? quant : 0,
                    DataCadastro = Data,
                    Condicao = Condicao,
                    DimensoesCm = double.TryParse(Comprimento, out var comp) ? comp : 0,
                    DimensoesLm = double.TryParse(Largura, out var larg) ? larg : 0,
                    Observacao = Observacoes,
                    Anexo = NomeArquivo, // Aqui você pode implementar a lógica para salvar o arquivo e obter o caminho real
                    Status = "Disponível"
                };

                // Executa o POST na rota da API: api/Residuo
                var response = await _httpClient.PostAsJsonAsync("api/Residuo", novoResiduo);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Resíduo cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    LimparCampos();
                }
                else
                {
                    var erroDetalhes = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Falha ao salvar: {response.StatusCode}\n{erroDetalhes}", "Erro API", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro de conexão: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SelecionarArquivo()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Todos os arquivos (*.*)|*.*",
                Title = "Selecione um arquivo para anexar"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                NomeArquivo = openFileDialog.FileName;
            }
        }

        private void LimparCampos()
        {
            TipoMaterial = null;
            Especificacao = string.Empty;
            Origem = null;
            ProjetoOrigem = string.Empty;
            Quantidade = string.Empty;
            Data = DateTime.Now;
            Condicao = null;
            Comprimento = string.Empty;
            Largura = string.Empty;
            Observacoes = string.Empty;
            NomeArquivo = "Nenhum arquivo selecionado";
        }
    }
}

