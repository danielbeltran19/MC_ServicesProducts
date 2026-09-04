using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Products.API.Middlewares;
using Products.Application;
using Products.Infrastructure;
using Products.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// --- Servicios ---
builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Products API - Prueba Técnica Fundación delamujer",
        Version = "v1",
        Description = "API REST para gestión de catálogo de productos y control de stock."
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// --- Migraciones automáticas al iniciar ---
// Práctico para el entorno de la prueba técnica / despliegue de demo.
// En un sistema productivo real esto normalmente se ejecuta como un paso
// explícito del pipeline de CI/CD, no en el arranque de la app.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
    dbContext.Database.Migrate();
}

// --- Pipeline HTTP ---
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API v1");
    options.RoutePrefix = "swagger";
});

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Necesario para que WebApplicationFactory<Program> funcione en pruebas de integración.
public partial class Program { }