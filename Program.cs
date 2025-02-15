using Restaurante.Repositories;
using Restaurante.Repositories.Interfaces;
using Restaurante.Services;
using Restaurante.Services.Interfaces;
using Microsoft.OpenApi.Models;
using CineAPI.Repositories.Interfaces;
using CineAPI.Repositories;
using CineAPI.Services.Interfaces;
using CineAPI.Services;

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

// Imprimir la cadena de conexión para depuración
Console.WriteLine($"DATABASE_URL: {postgresConnection}");

// **✅ Configuración de la base de datos**
if (databaseProvider == "PostgreSQL")
{
    Console.WriteLine("Usando PostgreSQL...");
    builder.Services.AddScoped<IProductosRepository>(provider =>
        new ProductosRepository(postgresConnection));
}

// **🔥 Registrar servicios y repositorios correctamente**
builder.Services.AddScoped<IProductosService, ProductosService>();
builder.Services.AddScoped<IUserRepository>(provider => new UserRepository(postgresConnection)); // ✅ Pasamos la conexión correctamente
builder.Services.AddScoped<IUserService, UserService>();

// **✅ Configuración de CORS corregida**
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173", builder =>
    {
        builder.WithOrigins("http://localhost:5173") // Asegura que sea el mismo puerto que usas en el front
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

// **✅ Configuración de Swagger**
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RestauranteAPI", Version = "v1" });
});

// **✅ Habilitar autenticación y autorización (si es necesario)**
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// **✅ Configurar controladores**
builder.Services.AddControllers();

var app = builder.Build();
app.UseCors("AllowLocalhost5173");

// **🌟 Aplicar Middleware en el ORDEN CORRECTO**
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestauranteAPI v1"));
}

app.UseRouting();
app.UseAuthentication();  
app.UseAuthorization();   
app.MapControllers();
app.Run();
