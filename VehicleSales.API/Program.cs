using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using VehicleSales.Domain.Interfaces;
using VehicleSales.Infrastructure.Data;
using VehicleSales.Infrastructure.Repositories;
using VehicleSales.Infrastructure.Seeders;
using VehicleSales.Application.Gateways;
using VehicleSales.Infrastructure.Gateways;
using VehicleSales.Application.Presenters;
using VehicleSales.Application.Controllers;

var builder = WebApplication.CreateBuilder(args);

// ===========================================
// CONFIGURAÇÃO DOS SERVIÇOS
// ===========================================

// 1. Controllers (SEM FluentValidation)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// 2. Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly("VehicleSales.Infrastructure");
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    });
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// 3. Repositories e Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

// 4. Clean Architecture - NOVOS SERVIÇOS
builder.Services.AddScoped<IVehicleGateway, VehicleGateway>();
builder.Services.AddScoped<IVehiclePresenter, VehiclePresenter>();
builder.Services.AddScoped<VehicleUseCaseController>();

// 5. Health Checks Simples
builder.Services.AddHealthChecks();

// 6. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vehicle Sales API",
        Version = "v1",
        Description = "API para gerenciamento de revenda de veículos automotores"
    });

    // Tentar incluir XML comments se existir
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// 7. CORS
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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vehicle API");
    c.RoutePrefix = "swagger";
});

// CORS
app.UseCors("AllowAll");

// Routing
app.UseRouting();

// Controllers
app.MapControllers();

// Health Check simples
app.MapHealthChecks("/health");

// ===========================================
// APLICAR MIGRATIONS E SEED
// ===========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Verificando banco de dados...");
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Tentar conectar com retry para Docker
        var retries = 0;
        var maxRetries = 10;
        
        while (retries < maxRetries)
        {
            try
            {
                // Criar banco se não existir e aplicar migrations
                context.Database.Migrate();
                
                logger.LogInformation("Banco de dados pronto!");
                
                // 🎲 SEED Data
                logger.LogInformation("Executando seed de dados...");
                await VehicleSeedData.SeedAsync(context);
                
                break;
            }
            catch (Exception)
            {
                retries++;
                if (retries == maxRetries) throw;
                
                logger.LogWarning($"Banco não disponível. Tentativa {retries}/{maxRetries}. Aguardando 5 segundos...");
                await Task.Delay(5000);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao conectar com o banco de dados!");
        // Continuar mesmo com erro para não travar o container
    }
}

app.Run();