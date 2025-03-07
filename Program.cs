using Restaurante.Repositories;
using Restaurante.Repositories.Interfaces;
using Restaurante.Services;
using Restaurante.Services.Interfaces;
using Microsoft.OpenApi.Models;
using CineAPI.Repositories.Interfaces;
using CineAPI.Repositories;
using CineAPI.Services.Interfaces;
using CineAPI.Services;
using Npgsql;
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8080");
// Cargar configuración
var configuration = builder.Configuration;
var postgresConnection = configuration.GetConnectionString("RestauranteDB_PostgreSQL");
// Verificar que la cadena de conexión no esté vacía
if (string.IsNullOrEmpty(postgresConnection))
{
    throw new InvalidOperationException("La cadena de conexión de la base de datos no está configurada. Verifica tu archivo appsettings.json.");
}
Console.WriteLine($"DATABASE_URL: {postgresConnection}");
// Configurar los repositorios con la conexión a PostgreSQL
builder.Services.AddScoped<IProductosRepository>(provider =>
    new ProductosRepository(postgresConnection));
builder.Services.AddScoped<IProductosService, ProductosService>();
builder.Services.AddScoped<IUserRepository>(provider =>
    new UserRepository(postgresConnection));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReservationRepository>(provider =>
    new ReservationRepository(postgresConnection));
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IPedidoRepository>(provider =>
    new PedidoRepository(postgresConnection));
builder.Services.AddScoped<IPedidoService, PedidoService>();
// 🔹 REGISTRO CORRECTO DE ADMINREPOSITORY PARA EVITAR EL ERROR 🔹
builder.Services.AddScoped<IAdminRepository>(provider =>
    new AdminRepository(postgresConnection));
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IItemsRepository>(provider =>
    new ItemsRepository(postgresConnection));
builder.Services.AddScoped<IItemsService, ItemsService>();
// Configurar CORS para permitir peticiones desde localhost:5173 (React/Vue/Angular en desarrollo)
builder.Services.AddCors(options =>
{
    options.AddPolicy("Elcors", builder =>
    {
        builder.WithOrigins("http://34.196.144.197")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
// Configurar Swagger para documentación de la API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RestauranteAPI", Version = "v1" });
});
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
var app = builder.Build();
// Habilitar CORS
app.UseCors("Elcors");
// Habilitar Swagger en TODOS los entornos (incluyendo producción)
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestauranteAPI v1"));
// Configurar middlewares de autenticación y autorización
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
// Mapear los controladores
app.MapControllers();
// Iniciar la aplicación
app.Run();