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

var configuration = builder.Configuration;

var databaseProvider = configuration["DatabaseProvider"] ?? "PostgreSQL";

var postgresConnection = configuration.GetConnectionString("RestauranteDB_PostgreSQL");

if (string.IsNullOrEmpty(postgresConnection))
{
    throw new InvalidOperationException("La cadena de conexión de la base de datos no está configurada. Verifica tu archivo appsettings.json.");
}

Console.WriteLine($"DATABASE_URL: {postgresConnection}");

if (databaseProvider == "PostgreSQL")
{
    Console.WriteLine("Usando PostgreSQL...");
    builder.Services.AddScoped<IProductosRepository>(provider =>
        new ProductosRepository(postgresConnection));
}

builder.Services.AddScoped<IProductosService, ProductosService>();
builder.Services.AddScoped<IUserRepository>(provider => new UserRepository(postgresConnection));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPedidoRepository>(provider => new PedidoRepository(postgresConnection));
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IItemsRepository>(provider => new ItemsRepository(postgresConnection));
builder.Services.AddScoped<IItemsService, ItemsService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RestauranteAPI", Version = "v1" });
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();
app.UseCors("AllowLocalhost5173");

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
