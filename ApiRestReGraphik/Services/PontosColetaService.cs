using ApiRestReGraphik.Models;
using ApiRestReGraphik.Repositories.Interface;
using Firebase.Database;

namespace ApiRestReGraphik.Services
{
    public class PontosColetaService
    {
        // Logger para registrar informações e erros relacionados ao serviço ReGraphik
        private readonly ILogger<PontosColetaService> _logger;
        private readonly IPontosColeta _repository;
        /// <summary>
        ///  Construtor da classe PontosColetaService que recebe as dependências necessárias, para permitir o registro de informações e erros durante a execução dos métodos do serviço.
        /// </summary>
        /// <param name="logger">Logger para registrar informações e erros</param>
        /// <param name="repository">Repositório para acessar os dados do PontosColeta</param>
        public PontosColetaService(ILogger<PontosColetaService> logger, IPontosColeta repository)
        {
            _logger = logger;
            _repository = repository;
        }

        /// <summary>
        /// Lista todos os pontos de coleta cadastrados no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao listar os dados</exception>
        public async Task<List<PontosColeta>> Listar()
        {
            try
            {
                return await _repository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao listar os dados do ponto de coleta: {ex.Message}");
                throw new Exception("Erro ao listar os dados do ponto de coleta");
            }
        }

        /// <summary>
        /// Obtém um ponto de coleta específico por ID, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="id">ID do ponto de coleta a ser obtido</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao obter o ponto de coleta por ID</exception>
        public async Task<PontosColeta> ObterPorId(string id)
        {
            try
            {
                return await _repository.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter o ponto de coleta por ID: {ex.Message}");
                throw new Exception("Erro ao obter o ponto de coleta por ID");
            }

        }

        /// <summary>
        /// Adiciona um novo ponto de coleta ao ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="pontosColeta">O ponto de coleta a ser adicionado</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao adicionar o ponto de coleta</exception>
        public async Task Criar(PontosColeta pontosColeta)
        {
            try
            {
                await _repository.Add(pontosColeta);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao adicionar o ponto de coleta: {ex.Message}");
                throw new Exception("Erro ao adicionar o ponto de coleta");
            }
        }

        /// <summary>
        /// Atualiza um ponto de coleta existente no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="pontosColeta">O ponto de coleta a ser atualizado</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao atualizar o ponto de coleta</exception>
        public async Task Atualizar(string id, PontosColeta pontosColeta)
        {
            try
            {
                await _repository.Update(id, pontosColeta);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar o ponto de coleta: {ex.Message}");
                throw new Exception("Erro ao atualizar o ponto de coleta");
            }
        }
        
        /// <summary>
        /// Exclui um ponto de coleta existente no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <param name="id">ID do ponto de coleta a ser excluído</param>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao excluir o ponto de coleta</exception>
        public async Task Excluir(string id)
        {
            try
            {
                await _repository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao excluir o ponto de coleta: {ex.Message}");
                throw new Exception("Erro ao excluir o ponto de coleta");
            }
        }
    }
}