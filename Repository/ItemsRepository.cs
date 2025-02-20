using Npgsql;
using Restaurante.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Repositories
{
    public class ItemsRepository : IItemsRepository
    {
        private readonly string _connectionString;

        public ItemsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Items>> GetAllAsync()
        {
            var items = new List<Items>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM Items", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new Items
                            {
                                IdDetalle = reader.GetInt32(0),
                                IdPedidos = reader.GetInt32(1),
                                IdProducto = reader.GetInt32(2),
                                Cantidad = reader.GetInt32(3),
                                Precio = reader.GetDecimal(4)
                            };
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
        }

        public async Task<Items> GetByIdAsync(int id)
        {
            Items item = null;
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM Items WHERE IdDetalle = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new Items
                            {
                                IdDetalle = reader.GetInt32(0),
                                IdPedidos = reader.GetInt32(1),
                                IdProducto = reader.GetInt32(2),
                                Cantidad = reader.GetInt32(3),
                                Precio = reader.GetDecimal(4)
                            };
                        }
                    }
                }
            }
            return item;
        }

        public async Task AddAsync(Items item)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("INSERT INTO Items (IdDetalle, IdPedidos, IdProducto, Cantidad, Precio) VALUES (@IdDetalle, @IdPedidos, @IdProducto, @Cantidad, @Precio)", connection))
                {
                    command.Parameters.AddWithValue("@IdDetalle", item.IdDetalle);
                    command.Parameters.AddWithValue("@IdPedidos", item.IdPedidos);
                    command.Parameters.AddWithValue("@IdProducto", item.IdProducto);
                    command.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                    command.Parameters.AddWithValue("@Precio", item.Precio);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateAsync(Items item)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("UPDATE Items SET IdPedidos = @IdPedidos, IdProducto = @IdProducto, Cantidad = @Cantidad, Precio = @Precio WHERE IdDetalle = @IdDetalle", connection))
                {
                    command.Parameters.AddWithValue("@IdDetalle", item.IdDetalle);
                    command.Parameters.AddWithValue("@IdPedidos", item.IdPedidos);
                    command.Parameters.AddWithValue("@IdProducto", item.IdProducto);
                    command.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                    command.Parameters.AddWithValue("@Precio", item.Precio);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("DELETE FROM Items WHERE IdDetalle = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
