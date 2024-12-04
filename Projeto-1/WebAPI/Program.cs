using Microsoft.OpenApi.Models;
using WebAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Habilitar CORS para permitir solicitações de qualquer origem
builder.Services.AddCors(options =>
{
    // Define uma política chamada "AllowAll" que permite qualquer origem, método e cabeçalho
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()  // Permite qualquer origem
               .AllowAnyMethod()  // Permite qualquer método HTTP (GET, POST, etc.)
               .AllowAnyHeader(); // Permite qualquer cabeçalho
    });
});

// Configuração do DbContext com SQLite
builder.Services.AddDbContext<WebAPIContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do JSON para controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configura o manipulador de referência para ignorar ciclos e referências circulares
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        // Ignora propriedades com valor null durante a serialização
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Configuração do Swagger para documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Define informações da documentação Swagger
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPI", Version = "v1" });
});

var app = builder.Build();

// Configura a URL de escuta da aplicação para aceitar conexões em todas as interfaces e porta 5245
builder.WebHost.UseUrls("http://0.0.0.0:5245");

// Habilita o uso da política de CORS definida anteriormente
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    // Em ambiente de desenvolvimento, habilita o Swagger para testes de API
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Define o endpoint do Swagger UI
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1");
    });
}

// Aplica automaticamente as migrações do banco de dados
using (var scope = app.Services.CreateScope())
{
    // Obtém o contexto do banco de dados
    var dbContext = scope.ServiceProvider.GetRequiredService<WebAPIContext>();

    // Aplica as migrações pendentes ao banco de dados
    dbContext.Database.Migrate();
}

app.UseAuthorization();

// Mapeia os controladores para o pipeline de requisições
app.MapControllers();

// Inicia a aplicação
app.Run();
