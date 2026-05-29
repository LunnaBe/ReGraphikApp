using ApiRestReGraphik.Models;
using Firebase.Database;
using Firebase.Database.Query;

namespace ApiRestReGraphik.Services
{
    public class UsuarioService
    {
        // Logger para registrar informações e erros relacionados ao serviço ReGraphik
        private readonly ILogger<UsuarioService> _logger;
        private readonly FirebaseClient _firebaseClient;
        private const string NodeName = "usuarios";

        /// <summary>
        ///  Construtor da classe UsuarioService que recebe as dependências necessárias, para permitir o registro de informações e erros durante a execução dos métodos do serviço.
        /// </summary>
        /// <param name="logger">Logger para registrar informações e erros</param>
        /// <param name="configuration">Configuração para acessar as variáveis de ambiente</param>
        public UsuarioService(ILogger<UsuarioService> logger, IConfiguration configuration) 
        {
            _logger = logger;
            var firebaseUrl = configuration["Firebase:RealtimeDatabaseUrl"];
            _firebaseClient = new FirebaseClient(firebaseUrl);
        }

        /// <summary>
        /// Lista todos os usuários cadastrados no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao listar os dados</exception>
        public async Task<List<Usuario>> Listar()
        {
            try
            {
                // Obtém os dados do Firebase para a coleção de usuários
                var usuarios = await _firebaseClient
                    .Child(NodeName)
                    .OnceAsync<Usuario>();

                // Mapeia os dados do Firebase para a lista de usuários
                return usuarios.Select(r => r.Object).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao listar os dados do Usuario: {ex.Message}");
                throw new Exception("Erro ao listar os dados do Usuario");
            }
        }

        /// <summary>
        /// Obtém um usuário específico por ID, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="id">ID do usuário a ser obtido</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao obter o usuário por ID</exception>
        public async Task<Usuario> ObterPorId(string id)
        {
            try
            {
                // Obtém um usuário do Firebase usando o ID fornecido
                var usuario = await _firebaseClient
                     .Child(NodeName)
                     .Child(id)
                     .OnceSingleAsync<Usuario>();   

                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter o usuário por ID: {ex.Message}");
                throw new Exception("Erro ao obter o usuário por ID");
            }

        }

        /// <summary>
        /// Adiciona um novo usuário ao ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="usuario">O usuário a ser adicionado</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao adicionar o usuário</exception>
        public async Task Criar(Usuario usuario)
        {
            try
            {
                if (string.IsNullOrEmpty(usuario.Id))
                {
                    usuario.Id = Guid.NewGuid().ToString(); // Garante que temos um ID único string
                }

                // Adiciona o usuário ao Firebase usando o ID como chave
                await _firebaseClient
                    .Child(NodeName)
                    .Child(usuario.Id)
                    .PutAsync(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao adicionar o usuário: {ex.Message}");
                throw new Exception("Erro ao adicionar o usuário");
            }
        }

        /// <summary>
        /// Atualiza um usuário existente no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="usuario">O usuário a ser atualizado</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao atualizar o usuário</exception>
        public async Task Atualizar(string id, Usuario usuario)
        {
            try
            {
                // Garante que o ID do usuário seja definido corretamente para a atualização
                usuario.Id = id;

                // Atualiza o usuário no Firebase usando o ID como chave
                await _firebaseClient
                    .Child(NodeName)
                    .Child(id)
                    .PutAsync(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar o usuário: {ex.Message}");
                throw new Exception("Erro ao atualizar o usuário");
            }
        }
        
        /// <summary>
        /// Exclui um usuário existente no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="id">ID do usuário a ser excluído</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao excluir o usuário</exception>
        public async Task Excluir(string id)
        {
            try
            {
                // Exclui o usuário do Firebase usando o ID fornecido
                await _firebaseClient
                    .Child(NodeName)
                    .Child(id)
                    .DeleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao excluir o usuário: {ex.Message}");
                throw new Exception("Erro ao excluir o usuário");
            }
        }

        /// <summary>
        /// Autentica um usuário com base no login e senha fornecidos, utilizando o repositório para acessar os dados e 
        /// registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="login">Login do usuário a ser autenticado.</param>
        /// <param name="senha">Senha do usuário a ser autenticado.</param>
        /// <returns>/returns>
        public async Task<Usuario?> Autenticar(string login, string senha)
        {
            try
            {
                // Obtém os dados do Firebase para a coleção de usuários
                var usuarios = await _firebaseClient
                .Child(NodeName)
                .OnceAsync<Usuario>();

                return usuarios
                    .Select(r => r.Object)
                    .FirstOrDefault(u => u.Login == login && u.Senha == senha);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao autenticar o usuário: {ex.Message}");
                throw new Exception("Erro ao autenticar o usuário");
            }
        }
    }
}