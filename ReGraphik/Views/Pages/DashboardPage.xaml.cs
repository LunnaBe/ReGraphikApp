using System.Windows.Controls;
using ReGraphik.ViewModels; // Adicione isso

namespace ReGraphik.Views.Pages
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();

            this.DataContext = new DashboardViewModel();
        }
    }
}