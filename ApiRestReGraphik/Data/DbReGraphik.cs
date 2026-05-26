using Firebase.Database;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace ApiRestReGraphik.Data
{
    public class DbReGraphik
    {
        // Inicializa a conexão com o Firebase Realtime Database
        public FirebaseClient DbFirebase { get; }

        // Construtor da classe que configura a conexão com o Firebase usando as credenciais e a URL do banco de dados fornecidas no arquivo de configuração
        [Obsolete]
        public DbReGraphik(IConfiguration configuration)
        {
            // Obtém o caminho do arquivo de credenciais do Firebase a partir do arquivo de configuração
            var credentialJson =
                configuration["Firebase:CredentialFilePath"];

            // Obtém o diretório de execução da aplicação
            var pastaExecucao =
                AppContext.BaseDirectory;

            // Combina o diretório de execução com o nome do arquivo de credenciais para obter o caminho completo
            var caminhoCompletoChave =
                Path.Combine(
                    pastaExecucao,
                    credentialJson);

            // Verifica se a instância do FirebaseApp já foi criada para evitar múltiplas inicializações
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential =
                        GoogleCredential.FromFile(
                            caminhoCompletoChave)
                });
            }

            // Configura a URL do Firebase Realtime Database a partir do arquivo de configuração
            DbFirebase = new FirebaseClient(configuration["Firebase:RealtimeDatabaseUrl"]);
        }


    }
}
