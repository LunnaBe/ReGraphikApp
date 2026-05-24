using ApiRestReGraphik.Data;
using ApiRestReGraphik.Models;
using ApiRestReGraphik.Repositories.Interface;
using Firebase.Database;
using Firebase.Database.Query;

namespace ApiRestReGraphik.Repositories
{
    public class PontosColetaRepository : IPontosColeta
    {
        // Cliente do Firebase para acessar o Firebase Realtime Database
        private readonly FirebaseClient _firebaseClient;
        // Nome do nó no Firebase Realtime Database onde os pontos de coleta serão armazenados
        private const string ChildName = "pontosColeta";

        private readonly ILogger<PontosColetaRepository> _logger;

        public PontosColetaRepository(ILogger<PontosColetaRepository> logger, IConfiguration configuration)
        {
            var baseUrl = configuration["Firebase:RealtimeDatabaseUrl"];

            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException(nameof(baseUrl), "A URL do Firebase não foi encontrada no appsettings.json. Verifique a chave 'Firebase:RealtimeDatabaseUrl'.");
            }

            _firebaseClient = new FirebaseClient(baseUrl);
            _logger = logger;
            
        }

        /// <summary>
        /// Busca todos os pontos de coleta cadastrados no Firebase Realtime Database e retorna uma lista de objetos do tipo PontoColeta.
        /// </summary>
        /// <returns></returns>
        public async Task<List<PontosColeta>> GetAll()
        {
            try
            {
                // Realiza uma consulta assíncrona ao Firebase Realtime Database para obter todos os pontos de coleta
                // armazenados no nó "pontosColeta" e retorna uma lista de objetos do tipo PontosColeta.
                var result = await _firebaseClient
                    .Child(ChildName)
                    .OnceAsync<PontosColeta>();

                return result.Select(item =>
                {
                    var pontoColeta = item.Object;
                    if (pontoColeta != null)
                    {
                        pontoColeta.Id = item.Key; // Preenche o ID vindo do Firebase
                    }
                    return pontoColeta;
                }).Where(r => r != null).ToList()!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar pontos de coleta do Firebase Realtime Database: {ex.Message}");
                throw new Exception("Erro ao buscar pontos de coleta do Firebase Realtime Database.");
            }
        }

        /// <summary>
        ///  Busca um ponto de coleta específico no Firebase Realtime Database com base no ID fornecido e retorna um objeto do tipo PontoColeta correspondente.
        /// </summary>
        /// <param name="id">ID do ponto de coleta a ser buscado</param>
        /// <returns></returns>
        public async Task<PontosColeta> GetById(string id)
        {
            try
            {
                // Realiza uma consulta assíncrona ao Firebase Realtime Database para obter um ponto de coleta específico
                var result = await _firebaseClient
                    .Child(ChildName)
                    .Child(id.ToString())
                    .OnceSingleAsync<PontosColeta>();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar ponto de coleta com ID {id} do Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao buscar ponto de coleta com ID {id} do Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Adiciona um novo ponto de coleta ao Firebase Realtime Database.
        /// </summary>
        /// <param name="pontoColeta">Objeto PontosColeta a ser adicionado</param>
        /// <returns></returns>
        public async Task Add(PontosColeta pontoColeta)
        {
            try
            {
                // Realiza uma operação assíncrona para adicionar um novo ponto de coleta ao Firebase Realtime Database,
                await _firebaseClient
                    .Child(ChildName)
                    .PostAsync(pontoColeta);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao adicionar ponto de coleta ao Firebase Realtime Database: {ex.Message}");
                throw new Exception("Erro ao adicionar ponto de coleta ao Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Atualiza um ponto de coleta existente no Firebase Realtime Database com base no ID fornecido, substituindo os dados do ponto de coleta pelo objeto PontosColeta fornecido.
        /// </summary>
        /// <param name="id">ID do ponto de coleta a ser atualizado</param>
        /// <param name="pontoColeta">Objeto PontosColeta com os dados atualizados</param>
        /// <returns></returns>
        public async Task Update(string id, PontosColeta pontoColeta)
        {
            try
            {
                // Realiza uma operação assíncrona para atualizar um ponto de coleta existente no Firebase Realtime Database,
                await _firebaseClient
                    .Child(ChildName)
                    .Child(id.ToString())
                    .PutAsync(pontoColeta);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Erro ao atualizar ponto de coleta com ID {id} no Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao atualizar ponto de coleta com ID {id} no Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Deleta um ponto de coleta do Firebase Realtime Database com base no ID fornecido, removendo o nó correspondente ao ponto de coleta do banco de dados.
        /// </summary>
        /// <param name="id">ID do ponto de coleta a ser excluído</param>
        /// <returns></returns>
        public async Task Delete(string id)
        {
            try
            {
                // Realiza uma operação assíncrona para deletar um resíduo do Firebase Realtime Database,
                await _firebaseClient
                    .Child(ChildName)
                    .Child(id.ToString())
                    .DeleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao deletar ponto de coleta com ID {id} do Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao deletar ponto de coleta com ID {id} do Firebase Realtime Database.");
            }
        }
    }
}

