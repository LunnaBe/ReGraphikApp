using ApiRestReGraphik.Models;
using ApiRestReGraphik.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiRestReGraphik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SugestaoResiduosController : ControllerBase
    {
        private readonly SugestaoResiduosService _sugestaoService;
        private readonly ILogger<SugestaoResiduosController> _logger;

        /// <summary>
        /// Construtor da classe SugestaoResiduosController, que recebe um logger e um serviço de Sugestao de residuos para ser utilizado nas ações do controlador.
        /// </summary>
        /// <param name="logger">Logger para registrar informações e erros.</param>
        /// <param name="sugestaoService">Serviço de Sugestao de residuos para operações relacionadas.</param>
        public SugestaoResiduosController(ILogger<SugestaoResiduosController> logger, SugestaoResiduosService sugestaoService)
        {
            _logger = logger;
            _sugestaoService = sugestaoService;
        }


        /// <summary>
        ///  GET api/SugestaoResiduos - Obtém dados do Sugestao de residuos e retorna uma lista de sugestões cadastradas no ReGraphik.
        /// </summary>
        /// 
        /// <remarks>Responsável por listar os dados do Sugestao. Retornando uma coleção de objetos detalhando informações técnicas e operacionais de cada sugestão, 
        /// com atributos como ID, id de cadastro do residuo, id de sugestão e data de aplicação da sugestão.
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
                _logger.LogError($"Erro ao obter dados do Sugestao de residuos. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }


        }

        /// <summary>
        /// GET api/SugestaoResiduos/{id} - Obtém uma sugestão de resíduos específica do ReGraphik com base no ID fornecido.
        /// </summary>
        /// 
        /// <remarks>Responsável por obter uma sugestão de resíduos específica do ReGraphik com base no ID fornecido. 
        /// 
        /// Exemplo de resposta: 
        /// 
        /// {
        ///     "Id": "0d95265b-2757-424e-8ea9-445e8fd2a422",
        ///     "id_cadastro_residuo": "fcf84628-5c2d-4d55-9c59-4aebc59fd318",
        ///     "id_sugestao": "d02b73ff-9924-4a7f-9a74-f27779515f51",
        ///     "data_aplicacao": "2026-05-29T18:57:11.912Z"
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
                    return NotFound($"Sugestão de resíduos com ID {id} não encontrada.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter dados da Sugestão de resíduos com ID {id}. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }

        /// <summary>
        /// POST api/SugestaoResiduos - Criar uma nova sugestão de resíduos no ReGraphik.
        /// </summary>
        /// 
        /// <remarks>Responsável por criar uma nova sugestão de resíduos no ReGraphik.
        /// 
        /// Requisitos de validação:
        /// - Id: Deve ser um guid gerado automaticamente pelo sistema. (ex: "0d95265b-2757-424e-8ea9-445e8fd2a422")
        /// - IdCadastroResiduo: Deve ser um guid válido que corresponda a um cadastro de resíduo existente no ReGraphik. (ex: "0d952346b-2757-424e-567B-445e5785")
        /// - IdSugestao: Deve ser um guid válido que corresponda a uma sugestão existente no ReGraphik. (ex: "0d95265b-4566-424e-8ea9-445e866735")
        /// - DataAplicacao: Deve ser uma data e hora válida no formato ISO 8601 (ex: "2024-06-01T12:00:00Z") e não pode ser uma data futura.
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
        public async Task<IActionResult> Post([FromBody] SugestaoResiduo sugestao)
        {
            try
            {
                if (sugestao == null)
                {
                    return BadRequest("Sugestão de resíduos inválida.");
                }

                if (string.IsNullOrEmpty(sugestao.IdCadastroResiduo) || string.IsNullOrEmpty(sugestao.IdSugestao))
                {
                    return BadRequest("Os campos 'id_cadastro_residuo' e 'id_sugestao' são obrigatórios.");
                }

                if (!sugestao.DataAplicacao.HasValue)
                {
                    sugestao.DataAplicacao = DateTime.UtcNow;
                }

                await _sugestaoService.Criar(sugestao);

                return CreatedAtAction(nameof(GetById), new { id = sugestao.Id }, sugestao);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao criar dados da Sugestão de resíduos. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }

        /// <summary>
        /// PUT api/SugestaoResiduos/{id} - Atualizar uma sugestão de resíduos existente no ReGraphik com base no ID fornecido.
        /// </summary>
        /// 
        /// <remarks>Responsável por atualizar uma sugestão de resíduos existente no ReGraphik com base no ID fornecido.
        /// Observação: Retorna um status 200 OK com os dados da sugestão de resíduos atualizada, um status 400 Bad Request se a requisição for inválida,
        /// um status 404 Not Found se a sugestão de resíduos não for encontrada ou um status 500 Internal Server Error em caso de falha.</remarks>
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
        public async Task<IActionResult> Put(string id, [FromBody] SugestaoResiduo sugestao)
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
                    return NotFound($"Sugestão de resíduos com ID {id} não encontrada.");
                }

                await _sugestaoService.Atualizar(id, sugestao);
                return Ok($"Sugestão de resíduos com ID {id} atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar dados da Sugestão de resíduos com ID {id}. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }

        /// <summary>
        /// DELETE api/SugestaoResiduos/{id} - Excluir uma sugestão de resíduos do ReGraphik com base no ID fornecido. 
        /// </summary>
        /// 
        /// <remarks>Responsável por excluir uma sugestão de resíduos do ReGraphik com base no ID fornecido. 
        /// Observação: Retorna um status 200 OK se a exclusão for bem-sucedida, um status 404 Not Found se a sugestão de resíduos não for 
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
                    return NotFound($"Sugestão de resíduos com ID {id} não encontrada.");
                }

                await _sugestaoService.Excluir(id);
                return Ok($"Sugestão de resíduos com ID {id} excluída com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao excluir dados da Sugestão de resíduos com ID {id}. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }


    }
}