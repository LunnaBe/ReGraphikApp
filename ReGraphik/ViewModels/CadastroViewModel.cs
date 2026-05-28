using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ReGraphik.Commands;

namespace ReGraphik.ViewModels
{
    public class CadastroViewModel : BaseViewModel
    {
        private string _nome = "";
        private string _cpf = "";
        private string _email = "";
        private string _login = "";
        private string _senha = "";

        public string Nome
        {
            get => _nome;
            set { _nome = value; OnPropertyChanged(); }
        }

        public string CPF
        {
            get => _cpf;
            set { _cpf = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

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

        public ICommand CadastrarCommand { get; }

        public CadastroViewModel()
        {
            CadastrarCommand = new RelayCommand(async _ => await Cadastrar());
        }

        private async Task Cadastrar()
        {
            using var http = new HttpClient();

            var usuario = new
            {
                Nome,
                CPF,
                Email,
                Login,
                Senha,
                DataCadastro = DateTime.Now
            };

            var json = JsonSerializer.Serialize(usuario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await http.PostAsync("http://localhost:5000/api/usuario", content);

            if (response.IsSuccessStatusCode)
                MessageBox.Show("Cadastro realizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Erro ao cadastrar. Verifique os dados e tente novamente.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
