using ApiRestReGraphik.Models;
using Firebase.Database;
using Firebase.Database.Query;

namespace ApiRestReGraphik.Services
{
    public class PontosColetaService
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly ILogger<PontosColetaService> _logger;
        private const string NodeName = "pontos_coleta";

        /// <summary>
        /// Lista todos os pontos de coleta cadastrados no ReGraphik, utilizando o repositório para acessar os dados e registrando qualquer erro que possa ocorrer durante a operação.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao listar os dados</exception>
        public async Task<List<PontosColeta>> Listar()
        {
            try
            {
                // Obtém os pontos de coleta do Firebase
                var pontos = await _firebaseClient
                    .Child(NodeName)
                    .OnceAsync<PontosColeta>();

                // Mapeia os dados do Firebase para a lista de PontosColeta
                return pontos.Select(p => p.Object).ToList();
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
                // Obtém o ponto de coleta do Firebase usando o ID fornecido
                var ponto = await _firebaseClient
                     .Child(NodeName)
                     .Child(id)
                     .OnceSingleAsync<PontosColeta>();

                return ponto;
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
                if (string.IsNullOrEmpty(pontosColeta.Id))
                {
                    pontosColeta.Id = Guid.NewGuid().ToString(); // Garante que temos um ID único string
                }

                // Adiciona o ponto de coleta ao Firebase usando o ID como chave
                var resultado = await _firebaseClient
                                .Child("pontoscoleta")
                                .PostAsync(pontosColeta);

                pontosColeta.Id = resultado.Key;

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
                pontosColeta.Id = id;

                // Atualiza o ponto de coleta no Firebase usando o ID como chave
                await _firebaseClient
                    .Child(NodeName)
                    .Child(id)
                    .PutAsync(pontosColeta);
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
                // Exclui o ponto de coleta do Firebase usando o ID fornecido
                await _firebaseClient
                    .Child(NodeName)
                    .Child(id)
                    .DeleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao excluir o ponto de coleta: {ex.Message}");
                throw new Exception("Erro ao excluir o ponto de coleta");
            }
        }
    }
}