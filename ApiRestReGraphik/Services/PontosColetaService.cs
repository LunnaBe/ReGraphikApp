using ApiRestReGraphik.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Google.Apis.Auth.OAuth2;

namespace ApiRestReGraphik.Services
{
    public class PontosColetaService
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly ILogger<PontosColetaService> _logger;
        private const string NodeName = "pontos_coleta";

        /// <summary>
        /// Construtor da classe PontosColetaService, responsável por inicializar o cliente do Firebase e o logger para a classe,
        /// permitindo a comunicação com o Realtime Database do Firebase e o registro de logs para monitoramento e depuração.
        /// </summary>
        /// <param name="logger"></param>
        public PontosColetaService(ILogger<PontosColetaService> logger, IConfiguration configuration)
        {
            _logger = logger;

            var dbUrl = configuration["Firebase:RealtimeDatabaseUrl"];
            var credentialsFileName = configuration["Firebase:CredentialFilePath"] ?? "ReGraphikFirebaseKey.json";

            if (string.IsNullOrEmpty(dbUrl))
            {
                _logger.LogError("Erro crítico: URL do Realtime Database não encontrada no appsettings.json");
                throw new Exception("Configurações do Firebase ausentes.");
            }

            try
            {
                // Obtém o caminho físico correto onde a API está rodando no servidor
                var caminhoBase = AppContext.BaseDirectory;
                var caminhoCompletoChave = Path.Combine(caminhoBase, credentialsFileName);

                if (!File.Exists(caminhoCompletoChave))
                {
                    _logger.LogError($"Arquivo de credenciais não encontrado em: {caminhoCompletoChave}");
                    throw new FileNotFoundException($"O arquivo {credentialsFileName} precisa estar na raiz da API.");
                }

                // Inicializa a credencial usando diretamente o arquivo .json do Firebase
                GoogleCredential credenciais;
                using (var stream = new FileStream(caminhoCompletoChave, FileMode.Open, FileAccess.Read))
                {
                    credenciais = GoogleCredential.FromStream(stream)
                        .CreateScoped(new[] {
                    "https://www.googleapis.com/auth/userinfo.email",
                    "https://www.googleapis.com/auth/firebase.database"
                        });
                }

                _firebaseClient = new FirebaseClient(
                    dbUrl,
                    new FirebaseOptions
                    {
                        AuthTokenAsyncFactory = async () =>
                        {
                            // Obtém o token de acesso de forma assíncrona e segura
                            var token = await credenciais.UnderlyingCredential.GetAccessTokenForRequestAsync();
                            return token;
                        }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Falha fatal ao inicializar o FirebaseService: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Lista todos os pontos de coleta cadastrados no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao listar os dados</exception>
        public async Task<List<PontosColeta>> Listar()
        {
            try
            {
                // Obtém os pontos de coleta do Firebase
                var pontos = await _firebaseClient
                    .Child(NodeName)
                    .OnceAsync<PontosColeta>();

                // Mapeia os dados do Firebase para a lista de PontosColeta
                return pontos.Select(p => p.Object).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao listar os dados do ponto de coleta: {ex.Message}");
                throw new Exception("Erro ao listar os dados do ponto de coleta");
            }
        }

        /// <summary>
        /// Obtém um ponto de coleta específico por ID, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="id">ID do ponto de coleta a ser obtido</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao obter o ponto de coleta por ID</exception>
        public async Task<PontosColeta> ObterPorId(string id)
        {
            try
            {
                // Obtém o ponto de coleta do Firebase usando o ID fornecido
                var ponto = await _firebaseClient
                     .Child(NodeName)
                     .Child(id)
                     .OnceSingleAsync<PontosColeta>();

                return ponto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter o ponto de coleta por ID: {ex.Message}");
                throw new Exception("Erro ao obter o ponto de coleta por ID");
            }

        }

        /// <summary>
        /// Adiciona um novo ponto de coleta ao ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="pontosColeta">O ponto de coleta a ser adicionado</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao adicionar o ponto de coleta</exception>
        public async Task Criar(PontosColeta pontosColeta)
        {
            try
            {
                // Adiciona o ponto de coleta ao Firebase e obtém o resultado, que inclui a chave gerada para o novo ponto de coleta
                var resultado = await _firebaseClient
                                .Child(NodeName)
                                .PostAsync(pontosColeta);

                // Atribui o ID gerado pelo Firebase ao ponto de coleta
                pontosColeta.Id = resultado.Key;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao adicionar o ponto de coleta no Firebase: {ex.Message}");
                throw new Exception($"Erro ao adicionar o ponto de coleta: {ex.Message}");
            }
        }

        /// <summary>
        /// Método para criar um ponto de coleta utilizando o método Criar, mantendo a consistência na nomenclatura e facilitando a compreensão do código.
        /// </summary> 
        /// <param name="pontosColeta">O ponto de coleta a ser adicionado</param>
        /// <returns></returns>
        public async Task BlacklistCriar(PontosColeta pontosColeta) => await Criar(pontosColeta);

        /// <summary>
        /// Atualiza um ponto de coleta existente no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="pontosColeta">O ponto de coleta a ser atualizado</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao atualizar o ponto de coleta</exception>
        public async Task Atualizar(string id, PontosColeta pontosColeta)
        {
            try
            {
                pontosColeta.Id = id;

                // Atualiza o ponto de coleta no Firebase usando o ID como chave
                await _firebaseClient
                    .Child(NodeName)
                    .Child(id)
                    .PutAsync(pontosColeta);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar o ponto de coleta: {ex.Message}");
                throw new Exception("Erro ao atualizar o ponto de coleta");
            }
        }
        
        /// <summary>
        /// Exclui um ponto de coleta existente no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="id">ID do ponto de coleta a ser excluído</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao excluir o ponto de coleta</exception>
        public async Task Excluir(string id)
        {
            try
            {
                // Exclui o ponto de coleta do Firebase usando o ID fornecido
                await _firebaseClient
                    .Child(NodeName)
                    .Child(id)
                    .DeleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao excluir o ponto de coleta: {ex.Message}");
                throw new Exception("Erro ao excluir o ponto de coleta");
            }
        }
    }
}