using ApiRestReGraphik.Models;
using ApiRestReGraphik.Repositories.Interface;
using Firebase.Database;

namespace ApiRestReGraphik.Services
{
    public class UsuarioService
    {
        // Logger para registrar informações e erros relacionados ao serviço ReGraphik
        private readonly ILogger<UsuarioService> _logger;
        private readonly IUsuario _repository;
        /// <summary>
        ///  Construtor da classe UsuarioService que recebe as dependências necessárias, para permitir o registro de informações e erros durante a execução dos métodos do serviço.
        /// </summary>
        /// <param name="logger">Logger para registrar informações e erros</param>
        /// <param name="repository">Repositório para acessar os dados do Usuario</param>
        public UsuarioService(ILogger<UsuarioService> logger, IUsuario repository)
        {
            _logger = logger;
            _repository = repository;
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
                return await _repository.GetAll();
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
                return await _repository.GetById(id);
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
                await _repository.Add(usuario);
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
                await _repository.Update(id, usuario);
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
                await _repository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao excluir o usuário: {ex.Message}");
                throw new Exception("Erro ao excluir o usuário");
            }
        }
    }
}