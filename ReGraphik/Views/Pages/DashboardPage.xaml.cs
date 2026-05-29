using System.Windows.Controls;
using ReGraphik.ViewModels; // Adicione isso

namespace ReGraphik.Views.Pages
{
    public partial class DashboardPage : Page
    {
        public DashboardPage(string nomeUsuario = "Usuário")
        {
            InitializeComponent();
            TxtNomeUsuario.Text = nomeUsuario;
            this.DataContext = new DashboardViewModel();

            this.DataContext = new DashboardViewModel();
        }
    }
}