using System.Windows;
using System.Windows.Controls;
using ReGraphik.Views.Pages;

namespace ReGraphik.Views
{
    public partial class MainWindow : Window
    {
        private Button? _btnAtivo;

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new DashboardPage());
            _btnAtivo = BtnDashboard;
        }

        private void SetarNavAtivo(Button btn)
        {
            if (_btnAtivo != null)
                _btnAtivo.Style = (Style)FindResource("NavBtn");
            btn.Style = (Style)FindResource("NavBtnAtivo");
            _btnAtivo = btn;
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            SetarNavAtivo(BtnDashboard);
            MainFrame.Navigate(new DashboardPage());
        }

        private void Residuos_Click(object sender, RoutedEventArgs e)
        {
            SetarNavAtivo(BtnResiduos);
            MainFrame.Navigate(new ResiduosPage());
        }

        private void Pontos_Click(object sender, RoutedEventArgs e)
        {
            SetarNavAtivo(BtnPontos);
            MainFrame.Navigate(new PontosColetaPage());
        }

        private void Mapa_Click(object sender, RoutedEventArgs e)
        {
            SetarNavAtivo(BtnMapa);
            MainFrame.Navigate(new MapaPage());
        }

        private void Relatorios_Click(object sender, RoutedEventArgs e)
        {
            SetarNavAtivo(BtnRelatorios);
            MainFrame.Navigate(new RelatoriosPage());
        }
    }
}