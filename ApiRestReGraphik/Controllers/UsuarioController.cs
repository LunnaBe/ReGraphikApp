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

        public UsuarioController(ILogger<UsuarioController> logger, UsuarioService usuarioService)
        {
            _logger = logger;
            _usuarioService = usuarioService;
        }

        [HttpGet]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var result = await _usuarioService.ObterPorId(id);
                if (result == null)
                    return NotFound($"Usuário com ID {id} não encontrado.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter dados do Usuário com ID {id}. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            try
            {
                if (usuario == null)
                    return BadRequest("Usuário inválido.");

                await _usuarioService.Criar(usuario);
                return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao criar dados do Usuário. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var usuario = await _usuarioService.Autenticar(request.Login, request.Senha);

                if (usuario == null)
                    return Unauthorized("Login ou senha inválidos.");

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao autenticar usuário. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Usuario usuario)
        {
            try
            {
                if (usuario == null || id != usuario.Id)
                    return BadRequest($"ID do usuário inválido.");

                var existing = await _usuarioService.ObterPorId(id);
                if (existing == null)
                    return NotFound($"Usuário com ID {id} não encontrado.");

                await _usuarioService.Atualizar(id, usuario);
                return Ok($"Usuário com ID {id} atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar dados do Usuário com ID {id}. Erro:{ex.Message}");
                throw new Exception("Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existing = await _usuarioService.ObterPorId(id);
                if (existing == null)
                    return NotFound($"Usuário com ID {id} não encontrado.");

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

    public class LoginRequest
    {
        public string Login { get; set; } = "";
        public string Senha { get; set; } = "";
    }
}
