using ApiRestReGraphik.Models;
using ReGraphik.Commands;
using ReGraphik.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ReGraphik.ViewModels
{
    /// <summary>
    /// ViewModel da janela de sugestões de reaproveitamento de um resíduo.
    /// Busca as sugestões compatíveis via API, exibe-as e registra a aplicação
    /// gravando um SugestaoResiduo no Firebase através da API REST.
    /// </summary>
    public class SugestaoResiduoViewModel : BaseViewModel
    {
        // ─── Infraestrutura ────────────────────────────────────────────────
        private static readonly HttpClient _http = new();
        private const string UrlApiSugestoes = "https://webregraphik.runasp.net/api/Sugestao";
        private const string UrlApiSugestaoResiduo = "https://webregraphik.runasp.net/api/SugestaoResiduos";

        // ─── Resíduo alvo ─────────────────────────────────────────────────
        public Residuo Residuo { get; }

        // ─── Lista de sugestões compatíveis com o tipo do resíduo ─────────
        public ObservableCollection<Sugestao> Sugestoes { get; } = new();

        // ─── Sugestão selecionada pelo usuário na lista ───────────────────
        private Sugestao _sugestaoSelecionada;
        public Sugestao SugestaoSelecionada
        {
            get => _sugestaoSelecionada;
            set
            {
                _sugestaoSelecionada = value;
                OnPropertyChanged();
            }
        }

        // ─── Controle de carregamento (exibe spinner / mensagem) ──────────
        private bool _carregando;
        public bool Carregando
        {
            get => _carregando;
            set
            {
                _carregando = value;
                OnPropertyChanged();
            }
        }

        // ─── Mensagem de feedback ao usuário ─────────────────────────────
        private string _mensagem;
        public string Mensagem
        {
            get => _mensagem;
            set
            {
                _mensagem = value;
                OnPropertyChanged();
            }
        }

        // ─── Comandos ─────────────────────────────────────────────────────
        public ICommand AplicarSugestaoCommand { get; }

        // ─── Construtor ───────────────────────────────────────────────────
        public SugestaoResiduoViewModel(Residuo residuo)
        {
            Residuo = residuo ?? throw new ArgumentNullException(nameof(residuo));

            // Inicializa o comando com suporte a parâmetro (recebe a Sugestao selecionada)
            AplicarSugestaoCommand = new RelayCommand(
                async _ => await AplicarSugestaoAsync(),
                _ => SugestaoSelecionada != null && !Carregando
            );

            // Carrega as sugestões assim que a ViewModel for criada
            _ = CarregarSugestoesAsync();
        }

        // ─── Busca sugestões compatíveis com o tipo do resíduo ────────────
        private async Task CarregarSugestoesAsync()
        {
            Carregando = true;
            Mensagem = "Buscando sugestões...";

            try
            {
                var resposta = await _http.GetAsync(UrlApiSugestoes);

                if (!resposta.IsSuccessStatusCode)
                {
                    Mensagem = "Não foi possível buscar as sugestões. Verifique sua conexão.";
                    return;
                }

                var json = await resposta.Content.ReadAsStringAsync();
                var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var todas = JsonSerializer.Deserialize<List<Sugestao>>(json, opcoes) ?? new List<Sugestao>();
                
                // Filtra sugestões compatíveis com o tipo do resíduo selecionado.
                // A comparação é case-insensitive e aceita sugestões sem restrição de tipo (TipoResiduoAceito vazio).
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Sugestoes.Clear();
                    foreach (var s in todas)
                    {
                        bool semFiltro = string.IsNullOrWhiteSpace(s.TipoResiduoAceito);
                        bool tipoCompat = !string.IsNullOrWhiteSpace(Residuo.TipoResiduo) &&
                                            s.TipoResiduoAceito != null &&
                                            s.TipoResiduoAceito.Contains(
                                                Residuo.TipoResiduo,
                                                StringComparison.OrdinalIgnoreCase);
                        if (semFiltro || tipoCompat)
                            Sugestoes.Add(s);
                    }

                    Mensagem = Sugestoes.Count > 0
                        ? $"{Sugestoes.Count} sugestão(ões) encontrada(s) para \"{Residuo.TipoResiduo}\"."
                        : $"Nenhuma sugestão encontrada para o tipo \"{Residuo.TipoResiduo}\".";
                });
            }
            catch (Exception ex)
            {
                Mensagem = $"Erro ao carregar sugestões: {ex.Message}";
                System.Diagnostics.Debug.WriteLine("Erro ao carregar sugestões: " + ex.Message);
            }
            finally
            {
                Carregando = false;
            }
        }

        // ─── Registra a sugestão aplicada via POST em api/SugestaoResiduos ─
        private async Task AplicarSugestaoAsync()
        {
            if (SugestaoSelecionada == null) return;

            Carregando = true;
            Mensagem = "Aplicando sugestão...";

            try
            {
                // Monta o payload seguindo o modelo SugestaoResiduo da API
                var sugestaoResiduo = new
                {
                    id = Guid.NewGuid().ToString(),
                    id_cadastro_residuo = Residuo.Id,
                    id_sugestao = SugestaoSelecionada.Id,
                    data_aplicacao = DateTime.UtcNow
                };

                var body = JsonSerializer.Serialize(sugestaoResiduo);
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var resposta = await _http.PostAsync(UrlApiSugestaoResiduo, content);

                if (resposta.IsSuccessStatusCode)
                {
                    Mensagem = $"✅ Sugestão aplicada com sucesso ao resíduo \"{Residuo.TipoResiduo}\"!";
                    MessageBox.Show(
                        $"Sugestão aplicada com sucesso!\n\nResíduo: {Residuo.TipoResiduo}\nSugestão: {SugestaoSelecionada.DescricaoSugestao}",
                        "Sugestão Aplicada",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    var erro = await resposta.Content.ReadAsStringAsync();
                    Mensagem = $"Erro ao aplicar sugestão: {resposta.StatusCode}";
                    MessageBox.Show(
                        $"Não foi possível aplicar a sugestão.\nCódigo: {resposta.StatusCode}\nDetalhe: {erro}",
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Mensagem = $"Erro ao aplicar sugestão: {ex.Message}";
                System.Diagnostics.Debug.WriteLine("Erro ao aplicar sugestão: " + ex.Message);
                MessageBox.Show(
                    $"Erro inesperado ao aplicar a sugestão:\n{ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                Carregando = false;
            }
        }
    }
}
