using Npgsql;
using Restaurante.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Restaurante.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly string _connectionString;

        public PedidoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Pedido> GetAll()
        {
            var pedidos = new List<Pedido>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM pedidos", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pedido = new Pedido
                            {
                                IdPedidos = reader.GetInt32(reader.GetOrdinal("id_pedidos")),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                                UserID = reader.GetInt32(reader.GetOrdinal("user_id"))
                            };
                            pedidos.Add(pedido);
                        }
                    }
                }
            }

            // Obtener los items para cada pedido
            foreach (var pedido in pedidos)
            {
                pedido.Itmes = GetItemsByPedidoId(pedido.IdPedidos);
            }

            return pedidos;
        }

        public Pedido GetById(int id)
        {
            Pedido pedido = null;
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM pedidos WHERE id_pedidos = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pedido = new Pedido
                            {
                                IdPedidos = reader.GetInt32(reader.GetOrdinal("id_pedidos")),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                                UserID = reader.GetInt32(reader.GetOrdinal("user_id"))
                            };
                        }
                    }
                }
            }

            if (pedido != null)
            {
                pedido.Itmes = GetItemsByPedidoId(pedido.IdPedidos);
            }

            return pedido;
        }

        public List<Items> GetItemsByPedidoId(int pedidoId)
        {
            var items = new List<Items>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM items WHERE idpedidos = @PedidoId", connection))
                {
                    command.Parameters.AddWithValue("@PedidoId", pedidoId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new Items
                            {
                                IdDetalle = reader.GetInt32(reader.GetOrdinal("iddetalle")),
                                IdPedidos = reader.GetInt32(reader.GetOrdinal("idpedidos")),
                                IdProducto = reader.GetInt32(reader.GetOrdinal("idproducto")),
                                Cantidad = reader.GetInt32(reader.GetOrdinal("cantidad")),
                                Precio = reader.GetDecimal(reader.GetOrdinal("precio"))
                            };
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
        }

        public IEnumerable<Pedido> GetByUserId(int userId)
        {
            var pedidos = new List<Pedido>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM pedidos WHERE user_id = @UserId ORDER BY fecha DESC", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pedido = new Pedido
                            {
                                IdPedidos = reader.GetInt32(reader.GetOrdinal("id_pedidos")),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                                UserID = reader.GetInt32(reader.GetOrdinal("user_id"))
                            };
                            pedidos.Add(pedido);
                        }
                    }
                }
            }

            // Obtener los items para cada pedido
            foreach (var pedido in pedidos)
            {
                pedido.Itmes = GetItemsByPedidoId(pedido.IdPedidos);
            }

            return pedidos;
        }

        public void Add(Pedido pedido)
        {
            // Verificar que el usuario existe
            bool userExists = false;
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT COUNT(1) FROM users WHERE userid = @UserID", connection))
                {
                    command.Parameters.AddWithValue("@UserID", pedido.UserID);
                    userExists = Convert.ToInt64(command.ExecuteScalar()) > 0;
                }
            }
            
            if (!userExists)
            {
                throw new InvalidOperationException($"El usuario con ID {pedido.UserID} no existe.");
            }

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                
                // Usar transacción para asegurar la integridad de datos
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insertar el pedido
                        using (var command = new NpgsqlCommand("INSERT INTO pedidos (fecha, user_id) VALUES (@Fecha, @UserID) RETURNING id_pedidos", connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Fecha", pedido.Fecha);
                            command.Parameters.AddWithValue("@UserID", pedido.UserID);
                            
                            // Recuperar el ID generado
                            var id = (int)command.ExecuteScalar();
                            pedido.IdPedidos = id;
                        }

                        // Insertar los items del pedido si existen
                        if (pedido.Itmes != null && pedido.Itmes.Count > 0)
                        {
                            // Obtener el próximo valor de iddetalle disponible
                            int nextIdDetalle = 1;
                            using (var command = new NpgsqlCommand("SELECT COALESCE(MAX(iddetalle), 0) + 1 FROM items", connection, transaction))
                            {
                                var result = command.ExecuteScalar();
                                if (result != null && result != DBNull.Value)
                                {
                                    nextIdDetalle = Convert.ToInt32(result);
                                }
                            }
                            
                            int detailCounter = 0;
                            foreach (var item in pedido.Itmes)
                            {
                                // Asignar valores a propiedades requeridas
                                item.IdPedidos = pedido.IdPedidos;
                                item.IdDetalle = nextIdDetalle + detailCounter;  // Generar ID manualmente
                                detailCounter++;
                                
                                using (var command = new NpgsqlCommand(
                                    "INSERT INTO items (iddetalle, idpedidos, idproducto, cantidad, precio) VALUES (@IdDetalle, @IdPedidos, @IdProducto, @Cantidad, @Precio)", 
                                    connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@IdDetalle", item.IdDetalle);
                                    command.Parameters.AddWithValue("@IdPedidos", item.IdPedidos);
                                    command.Parameters.AddWithValue("@IdProducto", item.IdProducto);
                                    command.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                                    command.Parameters.AddWithValue("@Precio", item.Precio);
                                    
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error al crear pedido: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                        }
                        throw;
                    }
                }
            }
        }

        public void Update(Pedido pedido)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("UPDATE pedidos SET fecha = @Fecha, user_id = @UserID WHERE id_pedidos = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", pedido.IdPedidos);
                    command.Parameters.AddWithValue("@Fecha", pedido.Fecha);
                    command.Parameters.AddWithValue("@UserID", pedido.UserID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Primero eliminar items asociados
                        using (var command = new NpgsqlCommand("DELETE FROM items WHERE idpedidos = @Id", connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Id", id);
                            command.ExecuteNonQuery();
                        }

                        // Luego eliminar el pedido
                        using (var command = new NpgsqlCommand("DELETE FROM pedidos WHERE id_pedidos = @Id", connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Id", id);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}