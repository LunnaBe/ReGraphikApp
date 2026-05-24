using ApiRestReGraphik.Models;
using ApiRestReGraphik.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiRestReGraphik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SugestaoController : ControllerBase
    {
        private readonly SugestaoService _sugestaoService;
        private readonly ILogger<SugestaoController> _logger;

        /// <summary>
        /// Construtor da classe SugestaoController, que recebe um logger e um serviço de Sugestao para ser utilizado nas ações do controlador.
        /// </summary>
        /// <param name="logger">Logger para registrar informações e erros.</param>
        /// <param name="sugestaoService">Serviço de Sugestao para operações relacionadas.</param>
        public SugestaoController(ILogger<SugestaoController> logger, SugestaoService sugestaoService)
        {
            _logger = logger;
            _sugestaoService = sugestaoService;
        }


        /// <summary>
        ///  GET api/Sugestao - Obtém dados do Sugestao e retorna uma lista de sugestões cadastradas no ReGraphik.
        /// </summary>
        /// 
        /// <remarks>Responsável por listar os dados do Sugestao. Retornando uma coleção de objetos detalhando informações técnicas e operacionais de cada sugestão, 
        /// com atributos como ID, tipo de residuo aceito e descrição da sugestão.
        /// 
        /// Observação: Retorna um status 200 OK com os dados do ReGraphik ou um status 500 Internal Server Error em caso de falha.
        /// </remarks>
        /// 
        /// <response code="200">Retorna os dados do ReGraphik.</response>
        /// <response code="500">Ocorreu um erro ao processar a solicitação.</response>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _sugestaoService.Listar();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter dados do Sugestao. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }


        }

        /// <summary>
        /// GET api/Sugestao/{id} - Obtém uma sugestão específica do ReGraphik com base no ID fornecido.
        /// </summary>
        /// 
        /// <remarks>Responsável por obter uma sugestão específica do ReGraphik com base no ID fornecido. 
        /// 
        /// Exemplo de resposta: 
        /// 
        /// {
        ///     "Id": -NxYZ123456789,
        ///     "TipoResiduoAceito": "Papel",
        ///     "DescricaoSugestao": "Aceitamos papel reciclável, como papel A4 usado, jornais e revistas. Por favor, certifique-se de que o papel esteja limpo e seco antes de descartá-lo conosco."
        /// }
        /// 
        /// Observação: Retorna um status 200 OK com os dados da sugestão, um status 404 Not Found se a sugestão não for encontrada ou 
        /// um status 500 Internal Server Error em caso de falha.
        /// 
        /// </remarks>
        /// 
        /// <param name="id">ID da sugestão a ser obtida.</param>
        /// 
        /// <response code="200">Retorna os dados da sugestão solicitada.</response>
        /// <response code="404">Sugestão com o ID fornecido não encontrada.</response>
        /// <response code="500">Ocorreu um erro ao processar a solicitação.</response>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var result = await _sugestaoService.ObterPorId(id);
                if (result == null)
                {
                    return NotFound($"Sugestão com ID {id} não encontrada.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter dados da Sugestão com ID {id}. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }

        /// <summary>
        /// POSTapi/Sugestao - Criar uma nova sugestão no ReGraphik.
        /// </summary>
        /// 
        /// <remarks>Responsável por criar uma nova sugestão no ReGraphik.
        /// 
        /// Requisitos de validação:
        /// - TipoResiduoAceito: Deve ser uma string não vazia que descreva o tipo de resíduo aceito pela sugestão.
        /// - DescricaoSugestao: Deve ser uma string não vazia que forneça detalhes sobre a sugestão, incluindo informações sobre o tipo de resíduo aceito,
        /// 
        /// Observação: Retorna um status 201 Created com os dados da sugestão criada, um status 400 Bad Request se a requisição for inválida ou
        /// um status 500 Internal Server Error em caso de falha.
        /// </remarks>
        /// 
        /// <param name="sugestao">Objeto do tipo Sugestao a ser criado.</param>
        /// 
        /// <response code="201">Sugestão criada com sucesso.</response>
        /// <response code="400">Requisição inválida, sugestão não fornecida ou dados incorretos.</response>
        /// <response code="500">Ocorreu um erro ao processar a solicitação.</response>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] Sugestao sugestao)
        {
            try
            {
                if (sugestao == null)
                {
                    return BadRequest("Sugestão inválida.");
                }

                await _sugestaoService.Criar(sugestao);

                return CreatedAtAction(nameof(GetById), new { id = sugestao.Id }, sugestao);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao criar dados da Sugestão. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }

        /// <summary>
        /// PUT api/Sugestao/{id} - Atualizar uma sugestão existente no ReGraphik com base no ID fornecido.
        /// </summary>
        /// 
        /// <remarks>Responsável por atualizar uma sugestão existente no ReGraphik com base no ID fornecido.
        /// Observação: Retorna um status 200 OK com os dados da sugestão atualizada, um status 400 Bad Request se a requisição for inválida,
        /// um status 404 Not Found se a sugestão não for encontrada ou um status 500 Internal Server Error em caso de falha.</remarks>
        /// 
        /// <param name="id"></param>
        /// <param name="sugestao"></param>
        /// 
        /// <response code="200">Sugestão atualizada com sucesso.</response>
        /// <response code="400">Requisição inválida, sugestão não fornecida ou dados incorretos.</response>
        /// <response code="404">Sugestão com o ID fornecido não encontrada.</response>
        /// <response code="500">Ocorreu um erro ao processar a solicitação.</response>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(string id, [FromBody] Sugestao sugestao)
        {
            try
            {
                if (sugestao == null || id != sugestao.Id)
                {
                    return BadRequest($"ID da sugestão inválido.");
                }

                var existing = await _sugestaoService.ObterPorId(id);
                if (existing == null)
                {
                    return NotFound($"Sugestão com ID {id} não encontrada.");
                }

                await _sugestaoService.Atualizar(id, sugestao);
                return Ok($"Sugestão com ID {id} atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar dados da Sugestão com ID {id}. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }

        /// <summary>
        /// DELETE api/Sugestao/{id} - Excluir uma sugestão do ReGraphik com base no ID fornecido. 
        /// </summary>
        /// 
        /// <remarks>Responsável por excluir uma sugestão do ReGraphik com base no ID fornecido. 
        /// Observação: Retorna um status 200 OK se a exclusão for bem-sucedida, um status 404 Not Found se a sugestão não for 
        /// encontrada ou um status 500 Internal Server Error em caso de falha.</remarks>
        /// 
        /// <param name="id">ID do resíduo a ser excluído.</param>
        /// 
        /// <response code="200">Resíduo excluído com sucesso.</response>
        /// <response code="404">Resíduo com o ID fornecido não encontrado.</response>
        /// <response code="500">Ocorreu um erro ao processar a solicitação.</response>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existing = await _sugestaoService.ObterPorId(id);
                if (existing == null)
                {
                    return NotFound($"Sugestão com ID {id} não encontrada.");
                }

                await _sugestaoService.Excluir(id);
                return Ok($"Sugestão com ID {id} excluída com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao excluir dados da Sugestão com ID {id}. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }


    }
}