using System.Windows;

namespace ReGraphik.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FecharAbas()
        {
            DashboardGrid.Visibility = Visibility.Collapsed;
            ResiduosGrid.Visibility = Visibility.Collapsed;
            PontosGrid.Visibility = Visibility.Collapsed;
            MapaGrid.Visibility = Visibility.Collapsed;
            RelatoriosGrid.Visibility = Visibility.Collapsed;
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            FecharAbas();
            DashboardGrid.Visibility = Visibility.Visible;
        }

        private void Residuos_Click(object sender, RoutedEventArgs e)
        {
            FecharAbas();
            ResiduosGrid.Visibility = Visibility.Visible;
        }

        private void Pontos_Click(object sender, RoutedEventArgs e)
        {
            FecharAbas();
            PontosGrid.Visibility = Visibility.Visible;
        }

        private void Mapa_Click(object sender, RoutedEventArgs e)
        {
            FecharAbas();
            MapaGrid.Visibility = Visibility.Visible;
        }

        private void Relatorios_Click(object sender, RoutedEventArgs e)
        {
            FecharAbas();
            RelatoriosGrid.Visibility = Visibility.Visible;
        }
    }
}