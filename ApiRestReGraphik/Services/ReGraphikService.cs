using ApiRestReGraphik.Models;
using ApiRestReGraphik.Repositories.Interface;
using Firebase.Database;

namespace ApiRestReGraphik.Services
{
    public class ReGraphikService
    {
        // Logger para registrar informações e erros relacionados ao serviço ReGraphik
        private readonly ILogger<ReGraphikService> _logger;
        private readonly IReGraphikRepository _repository;

        /// <summary>
        ///  Construtor da classe ReGraphikService que recebe as dependências necessárias, para permitir o registro de informações e erros durante a execução dos métodos do serviço.
        /// </summary>
        /// <param name="logger">Logger para registrar informações e erros</param>
        /// <param name="repository">Repositório para acessar os dados do ReGraphik</param>
        public ReGraphikService(ILogger<ReGraphikService> logger, IReGraphikRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        /// <summary>
        /// Lista todos os resíduos cadastrados no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao listar os dados</exception>
        public async Task<List<Residuo>> Listar()
        {
            try
            {
                return await _repository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao listar os dados do ReGraphik: {ex.Message}");
                throw new Exception("Erro ao listar os dados do ReGraphik");
            }
        }

        /// <summary>
        /// Obtém um resíduo específico por ID, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="id">ID do resíduo a ser obtido</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao obter o resíduo por ID</exception>
        public async Task<Residuo> ObterPorId(int id)
        {
            try
            {
                return await _repository.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter o resíduo por ID: {ex.Message}");
                throw new Exception("Erro ao obter o resíduo por ID");
            }

        }

        /// <summary>
        /// Adiciona um novo resíduo ao ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="residuo">O resíduo a ser adicionado</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao adicionar o resíduo</exception>
        public async Task Adicionar(Residuo residuo)
        {
            try
            {
                await _repository.Add(residuo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao adicionar o resíduo: {ex.Message}");
                throw new Exception("Erro ao adicionar o resíduo");
            }
        }

        /// <summary>
        /// Atualiza um resíduo existente no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="residuo">O resíduo a ser atualizado</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao atualizar o resíduo</exception>
        public async Task Atualizar(int id, Residuo residuo)
        {
            try
            {
                await _repository.Update(id, residuo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar o resíduo: {ex.Message}");
                throw new Exception("Erro ao atualizar o resíduo");
            }
        }
        
        /// <summary>
        /// Exclui um resíduo existente no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="id">ID do resíduo a ser excluído</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao excluir o resíduo</exception>
        public async Task Excluir(int id)
        {
            try
            {
                await _repository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao excluir o resíduo: {ex.Message}");
                throw new Exception("Erro ao excluir o resíduo");
            }
        }
    }
}