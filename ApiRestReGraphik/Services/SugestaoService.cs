using ApiRestReGraphik.Models;
using ApiRestReGraphik.Repositories.Interface;
using Firebase.Database;

namespace ApiRestReGraphik.Services
{
    public class SugestaoService
    {
        // Logger para registrar informações e erros relacionados ao serviço ReGraphik
        private readonly ILogger<SugestaoService> _logger;
        private readonly ISugestao _repository;
        /// <summary>
        ///  Construtor da classe SugestaoService que recebe as dependências necessárias, para permitir o registro de informações e erros durante a execução dos métodos do serviço.
        /// </summary>
        /// <param name="logger">Logger para registrar informações e erros</param>
        /// <param name="repository">Repositório para acessar os dados do ReGraphik</param>
        public SugestaoService(ILogger<SugestaoService> logger, ISugestao repository)
        {
            _logger = logger;
            _repository = repository;
        }

        /// <summary>
        /// Lista todas as sugestões cadastradas no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao listar os dados</exception>
        public async Task<List<Sugestao>> Listar()
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
        /// Obtém uma sugestão específica por ID, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="id">ID da sugestão a ser obtida</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao obter a sugestão por ID</exception>
        public async Task<Sugestao> ObterPorId(string id)
        {
            try
            {
                return await _repository.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter a sugestão por ID: {ex.Message}");
                throw new Exception("Erro ao obter a sugestão por ID");
            }

        }

        /// <summary>
        /// Adiciona uma sugestão ao ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="residuo">O resíduo a ser adicionado</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao adicionar a sugestão</exception>
        public async Task Criar(Sugestao sugestao)
        {
            try
            {
                await _repository.Add(sugestao);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao adicionar a sugestão: {ex.Message}");
                throw new Exception("Erro ao adicionar a sugestão");
            }
        }

        /// <summary>
        /// Atualiza uma sugestão existente no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="sugestao">A sugestão a ser atualizada</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao atualizar a sugestão</exception>
        public async Task Atualizar(string id, Sugestao sugestao)
        {
            try
            {
                await _repository.Update(id, sugestao);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar a sugestão: {ex.Message}");
                throw new Exception("Erro ao atualizar a sugestão");
            }
        }
        
        /// <summary>
        /// Exclui uma sugestão existente no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="id">ID da sugestão a ser excluída</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao excluir a sugestão</exception>
        public async Task Excluir(string id)
        {
            try
            {
                await _repository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao excluir a sugestão: {ex.Message}");
                throw new Exception("Erro ao excluir a sugestão");
            }
        }
    }
}