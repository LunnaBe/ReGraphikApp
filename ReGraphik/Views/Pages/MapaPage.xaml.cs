using ReGraphik.Models;
using Microsoft.Win32;
using ReGraphik.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ReGraphik.Views.Pages
{
    public partial class MapaPage : Page
    {
        private readonly MapaViewModel _viewModel;

        public MapaPage()
        {
            DefinirEmulacaoNavegador();

            InitializeComponent();
            _viewModel = new MapaViewModel();
            DataContext = _viewModel;

            // Inscreve-se nos eventos da ViewModel para lidar com as ações solicitadas
            _viewModel.SolicitouFocoNoMapa += ViewModel_SolicitouFocoNoMapa;
            _viewModel.SolicitouNavegacaoMapa += ViewModel_SolicitouNavegacaoMapa;
        }

        private void ViewModel_SolicitouNavegacaoMapa(string caminhoArquivoHtml)
        {
            // Garante que a navegação do WebBrowser ocorra estritamente na UI Thread
            Dispatcher.Invoke(() =>
            {
                try
                {
                    if (MapaBrowser != null)
                    {
                        MapaBrowser.Navigate(new Uri(caminhoArquivoHtml));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erro crítico na navegação: " + ex.Message);
                }
            });
        }

        private void ViewModel_SolicitouFocoNoMapa(int indice)
        {
            // Também protege a chamada do JavaScript via Dispatcher
            Dispatcher.Invoke(() =>
            {
                try
                {
                    if (MapaBrowser != null)
                    {
                        MapaBrowser.InvokeScript("centralizarPonto", indice);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erro ao invocar script do mapa: " + ex.Message);
                }
            });
        }

        private void ListaPontos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaPontos.SelectedItem is PontosColeta pontoSelecionado)
            {
                _viewModel.FocarNoPonto(pontoSelecionado);
            }
        }

        private void MapaBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            _viewModel.NotificarMapaCarregado();

            // Oculta o placeholder assim que a primeira navegação real terminar
            if (MapaPlaceholder != null && e.Uri.Scheme == "file")
            {
                MapaPlaceholder.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        // Método para definir a emulação do navegador para IE 11
        private static void DefinirEmulacaoNavegador()
        {
            try
            {
                string nomeProcesso = System.IO.Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                if (nomeProcesso.EndsWith(".vshost.exe")) nomeProcesso = nomeProcesso.Replace(".vshost.exe", ".exe");

                using (var chave = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true))
                {
                    if (chave != null)
                    {
                        // 11001 (0x2AF9) = Internet Explorer 11 Edge Mode (Máxima compatibilidade no WPF)
                        chave.SetValue(nomeProcesso, 11001, RegistryValueKind.DWord);
                    }
                }
            }
            catch { }
        }
    }
}
