using ReGraphik.ViewModels;
using System.Windows.Controls;

namespace ReGraphik.Views.Pages
{
    public partial class EstoqueReversoPage : Page
    {
        public EstoqueReversoPage()
        {
            InitializeComponent();
            DataContext = new EstoqueReversoViewModel();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
