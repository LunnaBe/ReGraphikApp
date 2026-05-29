using System.Windows;
using System.Windows.Controls;
using ReGraphik.Views.Pages;

namespace ReGraphik.Views
{
    public partial class MainWindow : Window
    {
        private Button? _btnAtivo;
        private string _nomeUsuario = "Usuário";

        public MainWindow(string nomeUsuario = "Usuário")
        {
            InitializeComponent();
            _nomeUsuario = nomeUsuario;
            TxtNomeUsuario.Text = nomeUsuario;
            MainFrame.Navigate(new DashboardPage(nomeUsuario));
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
            MainFrame.Navigate(new DashboardPage(_nomeUsuario));
        }

        private void Residuos_Click(object sender, RoutedEventArgs e)
        {
            SetarNavAtivo(BtnResiduos);
            MainFrame.Navigate(new ResiduosPage());
        }

        private void EstoqueReverso_Click(object sender, RoutedEventArgs e)
        {
            SetarNavAtivo(BtnEstoqueReverso);
            MainFrame.Navigate(new EstoqueReversoPage());
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