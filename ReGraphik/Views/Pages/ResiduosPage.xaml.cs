using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace ReGraphik.Views.Pages
{
    public partial class ResiduosPage : Page
    {
        public ResiduosPage()
        {
            InitializeComponent();

            // Define a data atual como padrão no DatePicker
            DpData.SelectedDate = DateTime.Now;
        }

        private void BtnEscolherArquivo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivos de Mídia|*.mp4;*.png;*.jpg;*.jpeg|Todos os arquivos|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                TxtNomeArquivo.Text = openFileDialog.SafeFileName;
            }
        }

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            // Reseta os campos para o padrão
            CboTipoMaterial.SelectedIndex = 0;
            TxtEspecificacao.Text = string.Empty;
            CboOrigem.SelectedIndex = 0;
            TxtProjetoOrigem.Text = string.Empty;
            TxtQuantidade.Text = string.Empty;
            DpData.SelectedDate = DateTime.Now;
            CboCondicao.SelectedIndex = 0;
            TxtDimensaoX.Text = string.Empty;
            TxtDimensaoY.Text = string.Empty;
            TxtObservacoes.Text = string.Empty;
            TxtNomeArquivo.Text = "Nenhum arquivo selecionado";
        }

        private void BtnSalvarResiduo_Click(object sender, RoutedEventArgs e)
        {
            // Logica para extrair e tratar os dados coletados antes de enviar para a API
            string material = (CboTipoMaterial.SelectedItem as ComboBoxItem)?.Content.ToString();
            string quantidade = TxtQuantidade.Text;

            MessageBox.Show($"Resíduo ({material} - {quantidade}kg) validado! Pronto para enviar para o backend.",
                            "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CboCondicao_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TxtDimensaoX_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}