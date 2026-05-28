using ReGraphik.ViewModels;
using System.Windows;

namespace ReGraphik.Views.Pages
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }
    }
}