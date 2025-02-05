using Restaurante.Repositories;
using Restaurante.Repositories.Interfaces;
using Restaurante.Services;
using Restaurante.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Cargar configuración desde appsettings.json
var configuration = builder.Configuration;

// Leer el proveedor de base de datos
var databaseProvider = configuration["DatabaseProvider"] ?? "PostgreSQL"; // Por defecto PostgreSQL

// Leer la cadena de conexión desde appsettings.json
var postgresConnection = configuration.GetConnectionString("RestauranteDB_PostgreSQL");

// Verificar si la cadena de conexión es válida
if (string.IsNullOrEmpty(postgresConnection))
{
    throw new InvalidOperationException("La cadena de conexión de la base de datos no está configurada. Verifica tu archivo appsettings.json.");
}

// Imprimir la cadena de conexión para verificar si está bien cargada
Console.WriteLine($"DATABASE_URL: {postgresConnection}");

// Configurar los servicios según el proveedor seleccionado
if (databaseProvider == "PostgreSQL")
{
    Console.WriteLine("Usando PostgreSQL...");

    // Configuración del repositorio y servicios para películas
    builder.Services.AddScoped<IProductosRepository>(provider =>
        new ProductosRepository(postgresConnection));
}

// Registrar IMovieService y su implementación
builder.Services.AddScoped<IProductosService, ProductosService>();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Configuración de Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RestauranteAPI", Version = "v1" });
});

builder.Services.AddControllers();

var app = builder.Build();

// Configurar el pipeline de la aplicación
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestauantePI v1"));
}

app.UseRouting();

// Usar la política de CORS configurada
app.UseCors("AllowLocalhost5173");

app.UseAuthorization();

app.MapControllers();

app.Run();