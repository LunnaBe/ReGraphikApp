using ApiRestReGraphik.Models;
using ApiRestReGraphik.Repositories.Interface;
using Firebase.Database;

namespace ApiRestReGraphik.Services
{
    public class SugestaoResiduosService
    {
        // Logger para registrar informações e erros relacionados ao serviço ReGraphik
        private readonly ILogger<SugestaoResiduosService> _logger;
        private readonly ISugestaoResiduos _repository;
        /// <summary>
        ///  Construtor da classe SugestaoResiduosService que recebe as dependências necessárias, para permitir o registro de informações e erros durante a execução dos métodos do serviço.
        /// </summary>
        /// <param name="logger">Logger para registrar informações e erros</param>
        /// <param name="repository">Repositório para acessar os dados do ReGraphik</param>
        public SugestaoResiduosService(ILogger<SugestaoResiduosService> logger, ISugestaoResiduos repository)
        {
            _logger = logger;
            _repository = repository;
        }

        /// <summary>
        /// Lista todas as sugestões de resíduos cadastradas no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao listar os dados</exception>
        public async Task<List<SugestaoResiduo>> Listar()
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
        /// Obtém uma sugestão de resíduo específica por ID, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="id">ID da sugestão de resíduo a ser obtida</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao obter a sugestão de resíduo por ID</exception>
        public async Task<SugestaoResiduo> ObterPorId(string id)
        {
            try
            {
                return await _repository.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter a sugestão de resíduo por ID: {ex.Message}");
                throw new Exception("Erro ao obter a sugestão de resíduo por ID");
            }

        }

        /// <summary>
        /// Adiciona uma sugestão de resíduo ao ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="sugestao">A sugestão de resíduo a ser adicionada</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao adicionar a sugestão de resíduo</exception>
        public async Task Criar(SugestaoResiduo sugestao)
        {
            try
            {
                await _repository.Add(sugestao);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao adicionar a sugestão de resíduo: {ex.Message}");
                throw new Exception("Erro ao adicionar a sugestão de resíduo");
            }
        }

        /// <summary>
        /// Atualiza uma sugestão de resíduo existente no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="sugestao">A sugestão de resíduo a ser atualizada</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao atualizar a sugestão de resíduo</exception>
        public async Task Atualizar(string id, SugestaoResiduo sugestao)
        {
            try
            {
                await _repository.Update(id, sugestao);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar a sugestão de resíduo: {ex.Message}");
                throw new Exception("Erro ao atualizar a sugestão de resíduo");
            }
        }
        
        /// <summary>
        /// Exclui uma sugestão de resíduo existente no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="id">ID da sugestão de resíduo a ser excluída</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao excluir a sugestão de resíduo</exception>
        public async Task Excluir(string id)
        {
            try
            {
                await _repository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao excluir a sugestão de resíduo: {ex.Message}");
                throw new Exception("Erro ao excluir a sugestão de resíduo");
            }
        }
    }
}