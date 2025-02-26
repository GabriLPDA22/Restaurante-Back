using Npgsql;
using Restaurante.Repositories.Interfaces;

namespace Restaurante.Repositories
{
    public class ProductosRepository : IProductosRepository
    {
        private readonly string _connectionString;

        public ProductosRepository(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión no puede ser nula o vacía.");
            }

            _connectionString = connectionString;
        }

        public async Task AddAsync(Productos producto)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
                    INSERT INTO Productos (
                        Nombre, Descripcion, Precio, ImagenUrl, Categorias, Alergenos
                    ) 
                    VALUES (
                        @Nombre, @Descripcion, @Precio, @ImagenUrl, @Categorias, @Alergenos
                    )";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@Precio", producto.Precio);
                    command.Parameters.AddWithValue("@ImagenUrl", producto.ImagenUrl);
                    command.Parameters.AddWithValue("@Categorias", producto.Categorias);
                    command.Parameters.AddWithValue("@Alergenos", producto.Alergenos);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int productoID)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Productos WHERE Id = @Id";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", productoID);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IEnumerable<Productos>> GetAllAsync()
        {
            var productos = new List<Productos>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Id, Nombre, Descripcion, Precio, ImagenUrl, Categorias, Alergenos FROM Productos";
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        productos.Add(new Productos
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Descripcion = reader.GetString(2),
                            Precio = reader.GetDecimal(3),
                            ImagenUrl = reader.GetString(4),
                            Categorias = reader[5] as List<string> ?? new List<string>(),
                            Alergenos = reader[6] as List<string> ?? new List<string>()
                        });
                    }
                }
            }
            return productos;
        }

        public async Task<Productos?> GetByIdAsync(int productoID)
        {
            Productos? producto = null;
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Id, Nombre, Descripcion, Precio, ImagenUrl, Categorias, Alergenos FROM Productos WHERE Id = @Id";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", productoID);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            producto = new Productos
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                Precio = reader.GetDecimal(3),
                                ImagenUrl = reader.GetString(4),
                                Categorias = reader[5] as List<string> ?? new List<string>(),
                                Alergenos = reader[6] as List<string> ?? new List<string>()
                            };
                        }
                    }
                }
            }
            return producto;
        }

        public async Task UpdateAsync(Productos producto)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
                    UPDATE Productos SET 
                        Nombre = @Nombre, 
                        Descripcion = @Descripcion, 
                        Precio = @Precio, 
                        ImagenUrl = @ImagenUrl, 
                        Categorias = @Categorias, 
                        Alergenos = @Alergenos
                    WHERE Id = @Id";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@Precio", producto.Precio);
                    command.Parameters.AddWithValue("@ImagenUrl", producto.ImagenUrl);
                    command.Parameters.AddWithValue("@Categorias", producto.Categorias);
                    command.Parameters.AddWithValue("@Alergenos", producto.Alergenos);
                    command.Parameters.AddWithValue("@Id", producto.Id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}