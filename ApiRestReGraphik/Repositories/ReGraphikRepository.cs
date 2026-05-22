using ApiRestReGraphik.Data;
using ApiRestReGraphik.Models;
using ApiRestReGraphik.Repositories.Interface;
using Firebase.Database;
using Firebase.Database.Query;

namespace ApiRestReGraphik.Repositories
{
    public class ReGraphikRepository : IReGraphikRepository
    {
        // Cliente do Firebase para acessar o Firebase Realtime Database
        private readonly FirebaseClient _firebaseClient;
        // Nome do nó no Firebase Realtime Database onde os resíduos serão armazenados
        private const string ChildName = "residuos";

        private readonly ILogger<ReGraphikRepository> _logger;

        public ReGraphikRepository(ILogger<ReGraphikRepository> logger)
        {
            _logger = logger;
            _firebaseClient = new FirebaseClient("Firebase:RealtimeDatabaseUrl");
        }

        /// <summary>
        /// Busca todos os resíduos cadastrados no Firebase Realtime Database e retorna uma lista de objetos do tipo Residuo.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Residuo>> GetAll()
        {
            try
            {
                // Realiza uma consulta assíncrona ao Firebase Realtime Database para obter todos os resíduos
                // armazenados no nó "residuos" e retorna uma lista de objetos do tipo Residuo.
                var result = await _firebaseClient
                    .Child(ChildName)
                    .OnceAsync<Residuo>();
                return result.Select(item => item.Object).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar resíduos do Firebase Realtime Database: {ex.Message}");
                throw new Exception("Erro ao buscar resíduos do Firebase Realtime Database.");
            }
        }

        /// <summary>
        ///  Busca um resíduo específico no Firebase Realtime Database com base no ID fornecido e retorna um objeto do tipo Residuo correspondente.
        /// </summary>
        /// <param name="id">ID do resíduo a ser buscado</param>
        /// <returns></returns>
        public async Task<Residuo> GetById(int id)
        {
            try
            {
                // Realiza uma consulta assíncrona ao Firebase Realtime Database para obter um resíduo específico
                var result = await _firebaseClient
                    .Child(ChildName)
                    .Child(id.ToString())
                    .OnceSingleAsync<Residuo>();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar resíduo com ID {id} do Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao buscar resíduo com ID {id} do Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Adiciona um novo resíduo ao Firebase Realtime Database.
        /// </summary>
        /// <param name="residuo">Objeto Residuo a ser adicionado</param>
        /// <returns></returns>
        public async Task Add(Residuo residuo)
        {
            try
            {
                // Realiza uma operação assíncrona para adicionar um novo resíduo ao Firebase Realtime Database,
                await _firebaseClient
                    .Child(ChildName)
                    .PostAsync(residuo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao adicionar resíduo ao Firebase Realtime Database: {ex.Message}");
                throw new Exception("Erro ao adicionar resíduo ao Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Atualiza um resíduo existente no Firebase Realtime Database com base no ID fornecido, substituindo os dados do resíduo pelo objeto Residuo fornecido.
        /// </summary>
        /// <param name="id">ID do resíduo a ser atualizado</param>
        /// <param name="residuo">Objeto Residuo com os dados atualizados</param>
        /// <returns></returns>
        public async Task Update(int id, Residuo residuo)
        {
            try
            {
                // Realiza uma operação assíncrona para atualizar um resíduo existente no Firebase Realtime Database,
                await _firebaseClient
                    .Child(ChildName)
                    .Child(id.ToString())
                    .PutAsync(residuo);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Erro ao atualizar resíduo com ID {id} no Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao atualizar resíduo com ID {id} no Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Deleta um resíduo do Firebase Realtime Database com base no ID fornecido, removendo o nó correspondente ao resíduo do banco de dados.
        /// </summary>
        /// <param name="id">ID do resíduo a ser excluído</param>
        /// <returns></returns>
        public async Task Delete(int id)
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
                _logger.LogError($"Erro ao deletar resíduo com ID {id} do Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao deletar resíduo com ID {id} do Firebase Realtime Database.");
            }
        }
    }
}

