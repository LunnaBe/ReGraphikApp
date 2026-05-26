using ApiRestReGraphik.Models;
using ApiRestReGraphik.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiRestReGraphik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PontosColetaController : ControllerBase
    {
        private readonly PontosColetaService _pontosColetaService;
        private readonly ILogger<PontosColetaController> _logger;

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Construtor da classe PontosColetaController, que recebe um logger e um serviço de PontosColeta para ser utilizado nas ações do controlador.
        /// </summary>
        /// <param name="logger">Logger para registrar informações e erros.</param>
        /// <param name="pontosColetaService">Serviço de PontosColeta para operações relacionadas.</param>
        public PontosColetaController(
            ILogger<PontosColetaController> logger,
            PontosColetaService pontosColetaService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration) // 2. Injete o IConfiguration aqui
        {
            _logger = logger;
            _pontosColetaService = pontosColetaService;
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration; // 3. Atribua o valor
        }


        /// <summary>
        ///  GET api/PontosColeta - Obtém dados diretamente da API externa do Google Maps.
        /// </summary>
        /// 
        /// <remarks>Responsável por listar os dados dos Pontos de Coleta. Retornando uma coleção de objetos detalhando informações técnicas e operacionais de cada ponto de coleta, 
        /// com atributos como ID, nome do ponto, cidade, estado, CEP e os tipos de resíduos aceitos.
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
                var result = await _pontosColetaService.Listar();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter dados dos Pontos de Coleta. Erro:{ex.Message}");
                return StatusCode(500, $"Erro ao processar a listagem dos pontos de coleta");
            }
        }

        /// <summary>
        /// GET api/PontosColeta/google?cidade=... - Busca pontos de coleta na API do Google Maps com base na cidade fornecida.
        /// </summary>
        [HttpGet("google")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFromGoogle([FromQuery] string cidade)
        {
            if (string.IsNullOrWhiteSpace(cidade))
            {
                return BadRequest("O parâmetro 'cidade' é obrigatório.");
            }

            try
            {
                // 1. 🛑 VALIDAÇÃO: Busca os pontos existentes no Firebase para validar a cidade
                var pontosNoBanco = await _pontosColetaService.Listar();

                bool cidadeExisteNoFirebase = pontosNoBanco.Any(p =>
                    p.Cidade != null &&
                    p.Cidade.Contains(cidade, StringComparison.OrdinalIgnoreCase));

                // Se NÃO existir a cidade no banco, barra a operação
                if (!cidadeExisteNoFirebase)
                {
                    return BadRequest($"A cidade '{cidade}' não está autorizada ou cadastrada no sistema.");
                }

                // 2. Se a cidade existe, busca os dados atualizados no Google Maps
                var listaGoogle = new List<PontosColeta>();
                var query = $"ponto de coleta reciclagem {cidade}";
                var apiKey = _configuration["GoogleMaps:ApiKey"];
                var url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={Uri.EscapeDataString(query)}&key={apiKey}";

                if (_httpClient == null)
                {
                    return StatusCode(500, "Erro de infraestrutura: HttpClient não injetado.");
                }

                var json = await _httpClient.GetStringAsync(url);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("results", out var results)) return Ok(listaGoogle);

                // 3. Itera sobre os resultados do Google, salva no Firebase e joga na lista
                foreach (var item in results.EnumerateArray())
                {
                    var nome = item.TryGetProperty("name", out var n) ? n.GetString() ?? "Sem nome" : "Sem nome";
                    var endereco = item.TryGetProperty("formatted_address", out var a) ? a.GetString() ?? cidade : cidade;

                    double latitudeReal = 0;
                    double longitudeReal = 0;
                    if (item.TryGetProperty("geometry", out var geometry) && geometry.TryGetProperty("location", out var location))
                    {
                        if (location.TryGetProperty("lat", out var latProp)) latitudeReal = latProp.GetDouble();
                        if (location.TryGetProperty("lng", out var lngProp)) longitudeReal = lngProp.GetDouble();
                    }

                    var tipos = "Reciclável";
                    if (item.TryGetProperty("types", out var typesEl) && typesEl.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        var typesList = typesEl.EnumerateArray().Select(t => t.GetString()).Where(t => !string.IsNullOrEmpty(t)).ToList();
                        if (typesList.Any(t => t.Contains("electronics", StringComparison.OrdinalIgnoreCase)))
                            tipos = "Eletrônicos";
                        else if (typesList.Any(t => t.Contains("hardware", StringComparison.OrdinalIgnoreCase)))
                            tipos = "Metal / Papel / Plástico";
                    }

                    var novoPonto = new PontosColeta
                    {
                        NomePonto = nome,
                        Cidade = endereco,
                        Estado = "BR",
                        CEP = "—",
                        ResiduosAceitos = tipos,
                        Lat = latitudeReal,
                        Lng = longitudeReal
                    };

                    try
                    {
                        // 🔥 ENVIA PARA O FIREBASE: Como a cidade está pré-autorizada, salva o ponto!
                        await _pontosColetaService.Criar(novoPonto);

                        // O método 'Criar' preenche automaticamente o novoPonto.Id com a chave gerada pelo Firebase
                        listaGoogle.Add(novoPonto);
                    }
                    catch (Exception exService)
                    {
                        // Se um ponto falhar o envio por rede, gera um ID temporário para o mapa não quebrar e continua
                        _logger.LogWarning($"Falha ao registrar ponto '{nome}' no Firebase: {exService.Message}");
                        novoPonto.Id = Guid.NewGuid().ToString();
                        listaGoogle.Add(novoPonto);
                    }
                }

                // Retorna a lista com os IDs oficiais gerados pelo Firebase para o WPF
                return Ok(listaGoogle);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro no processamento combinado: {ex.Message}");
                var erroDetalhado = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Erro Real no Backend: {erroDetalhado}");
            }
        }

        /// <summary>
        /// GET api/PontosColeta/{id} - Obtém um ponto de coleta específico do ReGraphik com base no ID fornecido.
        /// </summary>
        /// 
        /// <remarks>Responsável por obter um ponto de coleta específico do ReGraphik com base no ID fornecido. 
        /// 
        /// Exemplo de resposta: 
        /// 
        /// {
        ///     "Id": -NxYZ123456789,
        ///     "NomePonto": "Ponto de Coleta Central",
        ///     "Cidade": "São Paulo",
        ///     "Estado": "SP",
        ///     "Cep": "01000-000",
        ///     "ResiduosAceitos": "Papel, Plástico, Metal"
        /// }
        /// 
        /// Observação: Retorna um status 200 OK com os dados do resíduo, um status 404 Not Found se o resíduo não for encontrado ou 
        /// um status 500 Internal Server Error em caso de falha.
        /// 
        /// </remarks>
        /// 
        /// <param name="id">ID do ponto de coleta a ser obtido.</param>
        /// 
        /// <response code="200">Retorna os dados do ponto de coleta solicitado.</response>
        /// <response code="404">Ponto de coleta com o ID fornecido não encontrado.</response>
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
                var result = await _pontosColetaService.ObterPorId(id);
                if (result == null)
                {
                    return NotFound($"Ponto de coleta com ID {id} não encontrado.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter dados do Ponto de Coleta com ID {id}. Erro:{ex.Message}");
                return StatusCode(500, $"Erro ao processar a solicitação do ponto de coleta");
            }
        }

        /// <summary>
        /// POST api/PontosColeta - Criar um novo ponto de coleta no ReGraphik.
        /// </summary>
        /// 
        /// <remarks>Responsável por criar um novo ponto de coleta no ReGraphik.
        /// 
        /// Requisitos de validação:
        /// - NomePonto: Deve ser uma string não vazia. (ex: "Ponto de Coleta Central", "Ponto de Coleta Norte", etc.)
        /// - Cidade: Deve ser uma string não vazia. (ex: "São Paulo", "Rio de Janeiro", etc.)
        /// - Estado: Deve ser uma string não vazia. (ex: "SP", "RJ", etc.)
        /// - Cep: Deve ser uma string no formato de CEP brasileiro. (ex: "01000-000", "20000-000", etc.)
        /// - ResiduosAceitos: Deve ser uma string não vazia listando os tipos de resíduos aceitos. (ex: "Papel, Plástico, Metal", "Vidro, Eletrônicos", etc.)
        /// 
        /// Observação: Retorna um status 201 Created com os dados do ponto de coleta criado, um status 400 Bad Request se a requisição for inválida ou
        /// um status 500 Internal Server Error em caso de falha.
        /// </remarks>
        /// 
        /// <param name="pontoColeta">Objeto do tipo PontoColeta a ser criado.</param>
        /// 
        /// <response code="201">Ponto de coleta criado com sucesso.</response>
        /// <response code="400">Requisição inválida, ponto de coleta não fornecido ou dados incorretos.</response>
        /// <response code="500">Ocorreu um erro ao processar a solicitação.</response>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] PontosColeta pontoColeta)
        {
            try
            {
                if (pontoColeta == null)
                {
                    return BadRequest("Ponto de coleta inválido.");
                }

                await _pontosColetaService.Criar(pontoColeta);

                return CreatedAtAction(nameof(GetById), new { id = pontoColeta.Id }, pontoColeta);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao criar dados do Ponto de Coleta. Erro:{ex.Message}");
                return StatusCode(500, $"Erro ao processar a criação do ponto de coleta");
            }
        }

        /// <summary>
        /// PUT api/PontosColeta/{id} - Atualizar um ponto de coleta existente no ReGraphik com base no ID fornecido.
        /// </summary>
        /// 
        /// <remarks>Responsável por atualizar um ponto de coleta existente no ReGraphik com base no ID fornecido.
        /// Observação: Retorna um status 200 OK com os dados do ponto de coleta atualizado, um status 400 Bad Request se a requisição for inválida,
        /// um status 404 Not Found se o ponto de coleta não for encontrado ou um status 500 Internal Server Error em caso de falha.</remarks>
        /// 
        /// <param name="id"></param>
        /// <param name="pontoColeta"></param>
        /// 
        /// <response code="200">Ponto de coleta atualizado com sucesso.</response>
        /// <response code="400">Requisição inválida, ponto de coleta não fornecido ou dados incorretos.</response>
        /// <response code="404">Ponto de coleta com o ID fornecido não encontrado.</response>
        /// <response code="500">Ocorreu um erro ao processar a solicitação.</response>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(string id, [FromBody] PontosColeta pontoColeta)
        {
            try
            {
                if (pontoColeta == null || id != pontoColeta.Id)
                {
                    return BadRequest($"ID do ponto de coleta inválido.");
                }

                var existing = await _pontosColetaService.ObterPorId(id);
                if (existing == null)
                {
                    return NotFound($"Ponto de coleta com ID {id} não encontrado.");
                }

                await _pontosColetaService.Atualizar(id, pontoColeta);
                return Ok($"Ponto de coleta com ID {id} atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar dados do Ponto de Coleta com ID {id}. Erro:{ex.Message}");
                return StatusCode(500, $"Erro ao processar a atualização do ponto de coleta");
            }
        }

        /// <summary>
        /// DELETE api/PontosColeta/{id} - Excluir um ponto de coleta do ReGraphik com base no ID fornecido. 
        /// </summary>
        /// 
        /// <remarks>Responsável por excluir um ponto de coleta do ReGraphik com base no ID fornecido. 
        /// Observação: Retorna um status 200 OK se a exclusão for bem-sucedida, um status 404 Not Found se o ponto de coleta não for 
        /// encontrado ou um status 500 Internal Server Error em caso de falha.</remarks>
        /// 
        /// <param name="id">ID do ponto de coleta a ser excluído.</param>
        /// 
        /// <response code="200">Ponto de coleta excluído com sucesso.</response>
        /// <response code="404">Ponto de coleta com o ID fornecido não encontrado.</response>
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
                var existing = await _pontosColetaService.ObterPorId(id);
                if (existing == null)
                {
                    return NotFound($"Ponto de coleta com ID {id} não encontrado.");
                }

                await _pontosColetaService.Excluir(id);
                return Ok($"Ponto de coleta com ID {id} excluído com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao excluir dados do Ponto de Coleta com ID {id}. Erro:{ex.Message}");
                return StatusCode(500, $"Erro ao processar a exclusão do ponto de coleta");
            }
        }


    }
}