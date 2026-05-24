using ApiRestReGraphik.Repositories;
using ApiRestReGraphik.Repositories.Interface;
using ApiRestReGraphik.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Registra o repositório e o serviço do ReGraphik para que possam ser injetados em outros componentes da aplicação, como os controladores.
builder.Services.AddScoped<IPontosColeta, PontosColetaRepository>();
builder.Services.AddScoped<PontosColetaService>();

builder.Services.AddScoped<IResiduo, ResiduoRepository>();
builder.Services.AddScoped<ResiduoService>();

builder.Services.AddScoped<ISugestao, SugestaoRepository>();
builder.Services.AddScoped<SugestaoService>();

builder.Services.AddScoped<ISugestaoResiduos, SugestaoResiduoRepository>();
builder.Services.AddScoped<SugestaoResiduosService>();

builder.Services.AddScoped<IUsuario, UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();

// Configura o Swagger para incluir comentários XML, permitindo que as descrições dos endpoints sejam exibidas na documentação gerada.
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
});

// Configuração do CORS para permitir solicitações de qualquer origem.
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // Isso faz com que o Swagger seja a página inicial do seu site publicado
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API ReGraphik v1");
    c.RoutePrefix = string.Empty;
});

// Configuração do middleware de CORS para permitir solicitações de qualquer origem, o que é útil durante o desenvolvimento e testes,
// mas deve ser configurado com mais restrição em ambientes de produção para garantir a segurança da aplicação.
app.UseCors("PermitirTudo");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
