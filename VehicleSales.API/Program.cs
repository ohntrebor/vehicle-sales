using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using VehicleSales.Domain.Interfaces;
using VehicleSales.Infrastructure.Repositories;
using VehicleSales.Application.Gateways;
using VehicleSales.Application.Presenters;
using VehicleSales.Application.Controllers;
using VehicleSales.Infrastructure.Gateways;
using VehicleSales.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ===========================================
// CONFIGURAÇÃO DOS SERVIÇOS
// ===========================================

// 1. Controllers 
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// 2. MongoDB Configuration
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDb");
    return new MongoClient(connectionString);
});

builder.Services.AddScoped<IMongoDatabase>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    var databaseName = builder.Configuration.GetValue<string>("MongoDbSettings:DatabaseName");
    return client.GetDatabase(databaseName);
});

// 3. Repositories
builder.Services.AddScoped<IVehicleSaleRepository, VehicleSaleRepository>();

// 4. External Services
builder.Services.AddHttpClient<IVehicleCatalogService, VehicleCatalogService>(client =>
{
    var baseUrl = builder.Configuration.GetValue<string>("ExternalServices:VehicleCatalogApi");
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// 5. Clean Architecture Services
builder.Services.AddScoped<ISaleGateway, SaleGateway>();
builder.Services.AddScoped<ISalePresenter, SalePresenter>();
builder.Services.AddScoped<SaleUseCaseController>();

// 6. Health Checks
builder.Services.AddHealthChecks()
    .AddMongoDb(
        mongodbConnectionString: builder.Configuration.GetConnectionString("MongoDb"),
        name: "mongodb",
        timeout: TimeSpan.FromSeconds(5))
    .AddUrlGroup(
        uri: new Uri(builder.Configuration.GetValue<string>("ExternalServices:VehicleCatalogApi") + "/health"),
        name: "vehiclecatalog-api",
        timeout: TimeSpan.FromSeconds(5));

// 7. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vehicle Sales API",
        Version = "v1",
        Description = "API para gerenciamento de vendas de veículos"
    });

    // Incluir XML comments se existir
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// 8. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// ===========================================
// BUILD DA APLICAÇÃO
// ===========================================
var app = builder.Build();

// ===========================================
// PIPELINE DE MIDDLEWARE
// ===========================================

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vehicle Sales API");
    c.RoutePrefix = "swagger";
});

// CORS
app.UseCors("AllowAll");

// Routing
app.UseRouting();

// Controllers
app.MapControllers();

// Health Checks
app.MapHealthChecks("/health");

// ===========================================
// VERIFICAR CONEXÕES NO STARTUP
// ===========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Verificando conexão com MongoDB...");
        
        var mongoClient = services.GetRequiredService<IMongoClient>();
        var database = services.GetRequiredService<IMongoDatabase>();
        
        // Tentar conectar com retry para Docker
        var retries = 0;
        var maxRetries = 10;
        
        while (retries < maxRetries)
        {
            try
            {
                // Teste de conexão simples
                await mongoClient.ListDatabaseNamesAsync();
                
                logger.LogInformation("MongoDB conectado com sucesso!");
                logger.LogInformation($"Database: {database.DatabaseNamespace.DatabaseName}");
                
                break;
            }
            catch (Exception)
            {
                retries++;
                if (retries == maxRetries) throw;
                
                logger.LogWarning($"MongoDB não disponível. Tentativa {retries}/{maxRetries}. Aguardando 5 segundos...");
                await Task.Delay(5000);
            }
        }

        // Verificar conexão com VehicleCatalog API
        logger.LogInformation("Verificando conexão com Vehicle Catalog API...");
        var catalogService = services.GetRequiredService<IVehicleCatalogService>();
        
        // Teste simples de conectividade (não obrigatório)
        try
        {
            var httpClient = services.GetRequiredService<HttpClient>();
            var catalogApiUrl = builder.Configuration.GetValue<string>("ExternalServices:VehicleCatalogApi");
            var response = await httpClient.GetAsync($"{catalogApiUrl}/health");
            
            if (response.IsSuccessStatusCode)
                logger.LogInformation("Vehicle Catalog API disponível!");
            else
                logger.LogWarning("Vehicle Catalog API não está respondendo (continuando mesmo assim)");
        }
        catch (Exception ex)
        {
            logger.LogWarning($"Não foi possível conectar com Vehicle Catalog API: {ex.Message} (continuando mesmo assim)");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao verificar conexões!");
        // Para MongoDB, pode ser crítico, mas vamos continuar
    }
}

app.Run();