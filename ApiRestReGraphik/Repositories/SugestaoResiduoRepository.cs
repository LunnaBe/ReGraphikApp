using ApiRestReGraphik.Data;
using ApiRestReGraphik.Models;
using ApiRestReGraphik.Repositories.Interface;
using Firebase.Database;
using Firebase.Database.Query;

namespace ApiRestReGraphik.Repositories
{
    public class SugestaoResiduoRepository : ISugestaoResiduos
    {
        // Cliente do Firebase para acessar o Firebase Realtime Database
        private readonly FirebaseClient _firebaseClient;
        // Nome do nó no Firebase Realtime Database onde as sugestões de resíduos serão armazenadas
        private const string ChildName = "sugestoes_residuos";

        private readonly ILogger<SugestaoResiduoRepository> _logger;

        public SugestaoResiduoRepository(ILogger<SugestaoResiduoRepository> logger, IConfiguration configuration)
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
        /// Busca todas as sugestões de resíduos cadastradas no Firebase Realtime Database e retorna uma lista de objetos do tipo Sugestao.
        /// </summary>
        /// <returns></returns>
        public async Task<List<SugestaoResiduo>> GetAll()
        {
            try
            {
                // Realiza uma consulta assíncrona ao Firebase Realtime Database para obter todas as sugestões de resíduos
                // armazenadas no nó "residuos" e retorna uma lista de objetos do tipo SugestaoResiduo.
                var result = await _firebaseClient
                    .Child(ChildName)
                    .OnceAsync<SugestaoResiduo>();

                return result.Select(item =>
                {
                    var sugestaoResiduo = item.Object;
                    if (sugestaoResiduo != null)
                    {
                        sugestaoResiduo.Id = item.Key; // Preenche o ID vindo do Firebase
                    }
                    return sugestaoResiduo;
                }).Where(r => r != null).ToList()!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar sugestões de resíduos do Firebase Realtime Database: {ex.Message}");
                throw new Exception("Erro ao buscar sugestões de resíduos do Firebase Realtime Database.");
            }
        }

        /// <summary>
        ///  Busca uma sugestão de resíduo específica no Firebase Realtime Database com base no ID fornecido e retorna um objeto do tipo SugestaoResiduo correspondente.
        /// </summary>
        /// <param name="id">ID da sugestão de resíduo a ser buscada</param>
        /// <returns></returns>
        public async Task<SugestaoResiduo> GetById(string id)
        {
            try
            {
                // Realiza uma consulta assíncrona ao Firebase Realtime Database para obter uma sugestão específica
                var result = await _firebaseClient
                    .Child(ChildName)
                    .Child(id.ToString())
                    .OnceSingleAsync<SugestaoResiduo>();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar sugestão de resíduo com ID {id} do Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao buscar sugestão de resíduo com ID {id} do Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Adiciona uma nova sugestão de resíduo ao Firebase Realtime Database.
        /// </summary>
        /// <param name="sugestaoResiduo">Objeto SugestaoResiduo a ser adicionado</param>
        /// <returns></returns>
        public async Task Add(SugestaoResiduo sugestaoResiduo)
        {
            try
            {
                // Realiza uma operação assíncrona para adicionar uma nova sugestão ao Firebase Realtime Database,
                await _firebaseClient
                    .Child(ChildName)
                    .PostAsync(sugestaoResiduo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao adicionar sugestão de resíduo ao Firebase Realtime Database: {ex.Message}");
                throw new Exception("Erro ao adicionar sugestão de resíduo ao Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Atualiza uma sugestão de resíduo existente no Firebase Realtime Database com base no ID fornecido, substituindo os dados da sugestão pelo objeto SugestaoResiduo fornecido.
        /// </summary>
        /// <param name="id">ID da sugestão de resíduo a ser atualizada</param>
        /// <param name="sugestaoResiduo">Objeto SugestaoResiduo com os dados atualizados</param>
        /// <returns></returns>
        public async Task Update(string id, SugestaoResiduo sugestaoResiduo)
        {
            try
            {
                // Realiza uma operação assíncrona para atualizar uma sugestão existente no Firebase Realtime Database,
                await _firebaseClient
                    .Child(ChildName)
                    .Child(id.ToString())
                    .PutAsync(sugestaoResiduo);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Erro ao atualizar sugestão de resíduo com ID {id} no Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao atualizar sugestão de resíduo com ID {id} no Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Deleta uma sugestão de resíduo do Firebase Realtime Database com base no ID fornecido, removendo o nó correspondente à sugestão do banco de dados.
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
                _logger.LogError($"Erro ao deletar sugestão de resíduo com ID {id} do Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao deletar sugestão de resíduo com ID {id} do Firebase Realtime Database.");
            }
        }
    }
}

