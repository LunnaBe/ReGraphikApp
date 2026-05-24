using ApiRestReGraphik.Models;
using ApiRestReGraphik.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiRestReGraphik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly ILogger<UsuarioController> _logger;

        /// <summary>
        /// Construtor da classe UsuarioController, que recebe um logger e um serviço de Usuario para ser utilizado nas ações do controlador.
        /// </summary>
        /// <param name="logger">Logger para registrar informações e erros.</param>
        /// <param name="usuarioService">Serviço de Usuario para operações relacionadas.</param>
        public UsuarioController(ILogger<UsuarioController> logger, UsuarioService usuarioService)
        {
            _logger = logger;
            _usuarioService = usuarioService;
        }


        /// <summary>
        ///  GET api/Usuario - Obtém dados do Usuario e retorna uma lista de usuários cadastrados no ReGraphik.
        /// </summary>
        /// 
        /// <remarks>Responsável por listar os dados do Usuario. Retornando uma coleção de objetos detalhando informações técnicas e operacionais de cada usuário, 
        /// com atributos como ID, nome, cpf, email, login, senha e data de cadastro.
        /// 
        /// Observação: Retorna um status 200 OK com os dados do Usuario ou um status 500 Internal Server Error em caso de falha.
        /// </remarks>
        /// 
        /// <response code="200">Retorna os dados do Usuario.</response>
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
                var result = await _usuarioService.Listar();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter dados do Usuario. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }


        }

        /// <summary>
        /// GET api/Usuario/{id} - Obtém um usuário específico do Usuario com base no ID fornecido.
        /// </summary>
        /// 
        /// <remarks>Responsável por obter um usuário específico do ReGraphik com base no ID fornecido. 
        /// 
        /// Exemplo de resposta: 
        /// 
        /// {
        ///     "Id": -NxYZ123456789,
        ///     "Nome": "João da Silva",
        ///     "Cpf": "123.456.789-00",
        ///     "Email": "joao.silva@example.com",
        ///     "Login": "joaosilva",
        ///     "Senha": "********",
        ///     "DataCadastro": "2024-06-01T12:00:00"
        /// }
        /// 
        /// Observação: Retorna um status 200 OK com os dados do usuário, um status 404 Not Found se o usuário não for encontrado ou 
        /// um status 500 Internal Server Error em caso de falha.
        /// </remarks>
        /// 
        /// <param name="id">ID do usuário a ser obtido.</param>
        /// 
        /// <response code="200">Retorna os dados do usuário solicitado.</response>
        /// <response code="404">Usuário com o ID fornecido não encontrado.</response>
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
                var result = await _usuarioService.ObterPorId(id);
                if (result == null)
                {
                    return NotFound($"Usuário com ID {id} não encontrado.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter dados do Usuário com ID {id}. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }

        /// <summary>
        /// POST api/Usuario - Criar um novo usuário no ReGraphik.
        /// </summary>
        /// 
        /// <remarks>Responsável por criar um novo usuário no ReGraphik.
        /// 
        /// Requisitos de validação:
        /// - Nome: Deve ser uma string não vazia. (ex: "João da Silva", "Maria Oliveira", "Carlos Pereira", etc.)
        /// - Email: Deve ser um endereço de email válido. (ex: "joao.silva@example.com", "maria.oliveira@example.com", etc.)
        /// - Login: Deve ser uma string não vazia. (ex: "joaosilva", "mariaoliveira", etc.)
        /// - Senha: Deve ser uma string não vazia. (ex: "********", "********", etc.)
        /// - DataCadastro: Deve ser uma data válida. (ex: "2024-06-01T12:00:00", "2024-07-15T09:30:00", etc.)
        /// 
        /// 
        /// Observação: Retorna um status 201 Created com os dados do resíduo criado, um status 400 Bad Request se a requisição for inválida ou
        /// um status 500 Internal Server Error em caso de falha.
        /// </remarks>
        /// 
        /// <param name="usuario">Objeto do tipo Usuario a ser criado.</param>
        /// 
        /// <response code="201">Usuário criado com sucesso.</response>
        /// <response code="400">Requisição inválida, usuário não fornecido ou dados incorretos.</response>
        /// <response code="500">Ocorreu um erro ao processar a solicitação.</response>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            try
            {
                if (usuario == null)
                {
                    return BadRequest("Usuário inválido.");
                }

                await _usuarioService.Criar(usuario);

                return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao criar dados do Usuário. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }

        /// <summary>
        /// PUT api/Usuario/{id} - Atualizar um usuário existente no ReGraphik com base no ID fornecido.
        /// </summary>
        /// 
        /// <remarks>Responsável por atualizar um usuário existente no ReGraphik com base no ID fornecido.
        /// Observação: Retorna um status 200 OK com os dados do usuário atualizado, um status 400 Bad Request se a requisição for inválida,
        /// um status 404 Not Found se o usuário não for encontrado ou um status 500 Internal Server Error em caso de falha.</remarks>
        /// 
        /// <param name="id"></param>
        /// <param name="usuario"></param>
        /// 
        /// <response code="200">Usuário atualizado com sucesso.</response>
        /// <response code="400">Requisição inválida, usuário não fornecido ou dados incorretos.</response>
        /// <response code="404">Usuário com o ID fornecido não encontrado.</response>
        /// <response code="500">Ocorreu um erro ao processar a solicitação.</response>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(string id, [FromBody] Usuario usuario)
        {
            try
            {
                if (usuario == null || id != usuario.Id)
                {
                    return BadRequest($"ID do usuário inválido.");
                }

                var existing = await _usuarioService.ObterPorId(id);
                if (existing == null)
                {
                    return NotFound($"Usuário com ID {id} não encontrado.");
                }

                await _usuarioService.Atualizar(id, usuario);
                return Ok($"Usuário com ID {id} atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar dados do Usuário com ID {id}. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }

        /// <summary>
        /// DELETE api/Usuario/{id} - Excluir um usuário do ReGraphik com base no ID fornecido. 
        /// </summary>
        /// 
        /// <remarks>Responsável por excluir um usuário do ReGraphik com base no ID fornecido. 
        /// Observação: Retorna um status 200 OK se a exclusão for bem-sucedida, um status 404 Not Found se o usuário não for 
        /// encontrado ou um status 500 Internal Server Error em caso de falha.</remarks>
        /// 
        /// <param name="id">ID do usuário a ser excluído.</param>
        /// 
        /// <response code="200">Usuário excluído com sucesso.</response>
        /// <response code="404">Usuário com o ID fornecido não encontrado.</response>
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
                var existing = await _usuarioService.ObterPorId(id);
                if (existing == null)
                {
                    return NotFound($"Usuário com ID {id} não encontrado.");
                }

                await _usuarioService.Excluir(id);
                return Ok($"Usuário com ID {id} excluído com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao excluir dados do Usuário com ID {id}. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }


    }
}