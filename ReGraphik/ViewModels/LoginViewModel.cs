using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ReGraphik.Commands;
using ReGraphik.Views.Pages;
using ReGraphik.Views;
using ReGraphik.Models;

namespace ReGraphik.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _login = "";
        private string _senha = "";

        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        public string Senha
        {
            get => _senha;
            set { _senha = value; OnPropertyChanged(); }
        }

        public ICommand EntrarCommand { get; }

        public LoginViewModel()
        {
            EntrarCommand = new RelayCommand(async _ => await Entrar());
        }

        private async Task Entrar()
        {
            using var http = new HttpClient();

            var json = JsonSerializer.Serialize(new { Login, Senha });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await http.PostAsync("https://webregraphik.runasp.net/api/usuario/login", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResposta = await response.Content.ReadAsStringAsync();
                var usuario = JsonSerializer.Deserialize<Usuario>(jsonResposta, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                new MainWindow(usuario?.Nome ?? "Usuário").Show();
                foreach (Window w in Application.Current.Windows)
                {
                    if (w is LoginWindow)
                    {
                        w.Close();
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Login ou senha inválidos.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}