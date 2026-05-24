using ApiRestReGraphik.Data;
using ApiRestReGraphik.Models;
using ApiRestReGraphik.Repositories.Interface;
using Firebase.Database;
using Firebase.Database.Query;

namespace ApiRestReGraphik.Repositories
{
    public class UsuarioRepository : IUsuario
    {
        // Cliente do Firebase para acessar o Firebase Realtime Database
        private readonly FirebaseClient _firebaseClient;
        // Nome do nó no Firebase Realtime Database onde os usuários serão armazenados
        private const string ChildName = "usuarios";

        private readonly ILogger<UsuarioRepository> _logger;

        public UsuarioRepository(ILogger<UsuarioRepository> logger, IConfiguration configuration)
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
        /// Busca todos os usuários cadastrados no Firebase Realtime Database e retorna uma lista de objetos do tipo Usuario.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Usuario>> GetAll()
        {
            try
            {
                // Realiza uma consulta assíncrona ao Firebase Realtime Database para obter todos os usuários
                // armazenados no nó "usuarios" e retorna uma lista de objetos do tipo Usuario.
                var result = await _firebaseClient
                    .Child(ChildName)
                    .OnceAsync<Usuario>();

                return result.Select(item =>
                {
                    var usuario = item.Object;
                    if (usuario != null)
                    {
                        usuario.Id = item.Key; // Preenche o ID vindo do Firebase
                    }
                    return usuario;
                }).Where(r => r != null).ToList()!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar usuários do Firebase Realtime Database: {ex.Message}");
                throw new Exception("Erro ao buscar usuários do Firebase Realtime Database.");
            }
        }

        /// <summary>
        ///  Busca um usuário específico no Firebase Realtime Database com base no ID fornecido e retorna um objeto do tipo Usuario correspondente.
        /// </summary>
        /// <param name="id">ID do usuário a ser buscado</param>
        /// <returns></returns>
        public async Task<Usuario> GetById(string id)
        {
            try
            {
                // Realiza uma consulta assíncrona ao Firebase Realtime Database para obter um usuário específico
                var result = await _firebaseClient
                    .Child(ChildName)
                    .Child(id.ToString())
                    .OnceSingleAsync<Usuario>();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar usuário com ID {id} do Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao buscar usuário com ID {id} do Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Adiciona um novo usuário ao Firebase Realtime Database.
        /// </summary>
        /// <param name="usuario">Objeto Usuario a ser adicionado</param>
        /// <returns></returns>
        public async Task Add(Usuario usuario)
        {
            try
            {
                // Realiza uma operação assíncrona para adicionar um novo usuário ao Firebase Realtime Database,
                await _firebaseClient
                    .Child(ChildName)
                    .PostAsync(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao adicionar usuário ao Firebase Realtime Database: {ex.Message}");
                throw new Exception("Erro ao adicionar usuário ao Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Atualiza um usuário existente no Firebase Realtime Database com base no ID fornecido, substituindo os dados do usuário pelo objeto Usuario fornecido.
        /// </summary>
        /// <param name="id">ID do usuário a ser atualizado</param>
        /// <param name="usuario">Objeto Usuario com os dados atualizados</param>
        /// <returns></returns>
        public async Task Update(string id, Usuario usuario)
        {
            try
            {
                // Realiza uma operação assíncrona para atualizar um usuário existente no Firebase Realtime Database,
                await _firebaseClient
                    .Child(ChildName)
                    .Child(id.ToString())
                    .PutAsync(usuario);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Erro ao atualizar usuário com ID {id} no Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao atualizar usuário com ID {id} no Firebase Realtime Database.");
            }
        }

        /// <summary>
        /// Deleta um usuário do Firebase Realtime Database com base no ID fornecido, removendo o nó correspondente ao usuário do banco de dados.
        /// </summary>
        /// <param name="id">ID do usuário a ser excluído</param>
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
                _logger.LogError($"Erro ao deletar usuário com ID {id} do Firebase Realtime Database: {ex.Message}");
                throw new Exception($"Erro ao deletar usuário com ID {id} do Firebase Realtime Database.");
            }
        }
    }
}

