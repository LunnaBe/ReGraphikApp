using ApiRestReGraphik.Data;
using ApiRestReGraphik.Models;
using ApiRestReGraphik.Repositories.Interface;
using Firebase.Database;
using Firebase.Database.Query;

namespace ApiRestReGraphik.Repositories
{
    public class SugestaoRepository : ISugestao
    {
        // Cliente do Firebase para acessar o Firebase Realtime Database
        private readonly FirebaseClient _firebaseClient;
        // Nome do nó no Firebase Realtime Database onde as sugestões serão armazenadas
        private const string ChildName = "sugestoes";

        private readonly ILogger<SugestaoRepository> _logger;

        public SugestaoRepository(ILogger<SugestaoRepository> logger, IConfiguration configuration)
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
        /// Busca todas as sugestões cadastradas no Firebase Realtime Database e retorna uma lista de objetos do tipo Sugestao.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Sugestao>> GetAll()
        {
            try
            {
                // Realiza uma consulta assíncrona ao Firebase Realtime Database para obter todas as sugestões
                // armazenadas no nó "sugestoes" e retorna uma lista de objetos do tipo Sugestao.
                var result = await _firebaseClient
                    .Child(ChildName)
                    .OnceAsync<Sugestao>();

                return result.Select(item =>
                {
                    var sugestao = item.Object;
                    if (sugestao != null)
                    {
                        sugestao.Id = item.Key; // Preenche o ID vindo do Firebase
                    }
                    return sugestao;
                }).Where(r => r != null).ToList()!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar sugestões do Firebase Realtime Database: {ex.Message}");
                throw new Exception("Erro ao buscar sugestões do Firebase Realtime Database.");
            }
        }

        /// <summary>
        ///  Busca uma sugestão específica no Firebase Realtime Database com base no ID fornecido e retorna um objeto do tipo Sugestao correspondente.
        /// </summary>
        /// <param name="id">ID da sugestão a ser buscada</param>
        /// <returns></returns>
        public async Task<Sugestao> GetById(string id)
        {
            try
            {
                // Realiza uma consulta assíncrona ao Firebase Realtime Database para obter uma sugestão específica
                var result = await _firebaseClient
                    .Child(ChildName)
                    .Child(id.ToString())
                    .OnceSingleAsync<Sugestao>();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar sugestão com ID {id} do Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao buscar sugestão com ID {id} do Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Adiciona uma nova sugestão ao Firebase Realtime Database.
        /// </summary>
        /// <param name="sugestao">Objeto Sugestao a ser adicionado</param>
        /// <returns></returns>
        public async Task Add(Sugestao sugestao)
        {
            try
            {
                // Realiza uma operação assíncrona para adicionar uma nova sugestão ao Firebase Realtime Database,
                await _firebaseClient
                    .Child(ChildName)
                    .PostAsync(sugestao);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao adicionar sugestão ao Firebase Realtime Database: {ex.Message}");
                throw new Exception("Erro ao adicionar sugestão ao Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Atualiza uma sugestão existente no Firebase Realtime Database com base no ID fornecido, substituindo os dados da sugestão pelo objeto Sugestao fornecido.
        /// </summary>
        /// <param name="id">ID da sugestão a ser atualizada</param>
        /// <param name="sugestao">Objeto Sugestao com os dados atualizados</param>
        /// <returns></returns>
        public async Task Update(string id, Sugestao sugestao)
        {
            try
            {
                // Realiza uma operação assíncrona para atualizar uma sugestão existente no Firebase Realtime Database,
                await _firebaseClient
                    .Child(ChildName)
                    .Child(id.ToString())
                    .PutAsync(sugestao);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Erro ao atualizar sugestão com ID {id} no Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao atualizar sugestão com ID {id} no Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Deleta uma sugestão do Firebase Realtime Database com base no ID fornecido, removendo o nó correspondente à sugestão do banco de dados.
        /// </summary>
        /// <param name="id">ID da sugestão a ser excluída</param>
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
                _logger.LogError($"Erro ao deletar sugestão com ID {id} do Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao deletar sugestão com ID {id} do Firebase Realtime Database.");
            }
        }
    }
}

