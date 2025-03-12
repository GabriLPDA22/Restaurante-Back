using Npgsql;
using Restaurante.Repositories.Interfaces;
using System;
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

        public void Add(Pedido pedido)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Verificar que el usuario existe
                        using (var command = new NpgsqlCommand(
                            "SELECT COUNT(1) FROM users WHERE userid = @UserID", 
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@UserID", pedido.UserID);
                            var userCount = Convert.ToInt64(command.ExecuteScalar());
                            
                            if (userCount == 0)
                            {
                                throw new InvalidOperationException($"El usuario con ID {pedido.UserID} no existe.");
                            }
                        }

                        // Insertar el pedido
                        int pedidoId;
                        using (var command = new NpgsqlCommand(
                            "INSERT INTO pedidos (fecha, user_id) VALUES (@Fecha, @UserID) RETURNING id_pedidos", 
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Fecha", pedido.Fecha);
                            command.Parameters.AddWithValue("@UserID", pedido.UserID);
                            
                            pedidoId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // Insertar los items del pedido si existen
                        if (pedido.items != null && pedido.items.Count > 0)
                        {
                            // Obtener el próximo valor de iddetalle
                            int nextIdDetalle;
                            using (var command = new NpgsqlCommand(
                                "SELECT COALESCE(MAX(iddetalle), 0) + 1 FROM items", 
                                connection, transaction))
                            {
                                nextIdDetalle = Convert.ToInt32(command.ExecuteScalar());
                            }

                            foreach (var item in pedido.items)
                            {
                                // Usar el siguiente valor de iddetalle
                                string insertItemSql = @"
INSERT INTO items (iddetalle, idpedidos, idproducto, cantidad, precio) 
VALUES (@IdDetalle, @IdPedidos, @IdProducto, @Cantidad, @Precio)";

                                using (var command = new NpgsqlCommand(insertItemSql, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@IdDetalle", nextIdDetalle);
                                    command.Parameters.AddWithValue("@IdPedidos", pedidoId);
                                    command.Parameters.AddWithValue("@IdProducto", item.IdProducto);
                                    command.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                                    command.Parameters.AddWithValue("@Precio", item.Precio);
                                    
                                    command.ExecuteNonQuery();

                                    // Asignar el iddetalle generado al item
                                    item.IdDetalle = nextIdDetalle;
                                    
                                    // Incrementar para el próximo item
                                    nextIdDetalle++;
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new InvalidOperationException($"Error al crear el pedido: {ex.Message}", ex);
                    }
                }
            }
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
                            var pedido = new Pedido(
                                reader.GetInt32(reader.GetOrdinal("id_pedidos")),
                                reader.GetDateTime(reader.GetOrdinal("fecha")),
                                reader.GetInt32(reader.GetOrdinal("user_id"))
                            );
                            pedidos.Add(pedido);
                        }
                    }
                }
            }

            // Obtener los items para cada pedido
            foreach (var pedido in pedidos)
            {
                pedido.items = GetItemsByPedidoId(pedido.IdPedidos);
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
                            pedido = new Pedido(
                                reader.GetInt32(reader.GetOrdinal("id_pedidos")),
                                reader.GetDateTime(reader.GetOrdinal("fecha")),
                                reader.GetInt32(reader.GetOrdinal("user_id"))
                            );
                        }
                    }
                }
            }

            if (pedido != null)
            {
                pedido.items = GetItemsByPedidoId(pedido.IdPedidos);
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
                            var pedido = new Pedido(
                                reader.GetInt32(reader.GetOrdinal("id_pedidos")),
                                reader.GetDateTime(reader.GetOrdinal("fecha")),
                                reader.GetInt32(reader.GetOrdinal("user_id"))
                            );
                            pedidos.Add(pedido);
                        }
                    }
                }
            }

            // Obtener los items para cada pedido
            foreach (var pedido in pedidos)
            {
                pedido.items = GetItemsByPedidoId(pedido.IdPedidos);
            }

            return pedidos;
        }

        public void Update(Pedido pedido)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try 
                    {
                        // Actualizar el pedido
                        using (var command = new NpgsqlCommand(
                            "UPDATE pedidos SET fecha = @Fecha, user_id = @UserID WHERE id_pedidos = @Id", 
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Id", pedido.IdPedidos);
                            command.Parameters.AddWithValue("@Fecha", pedido.Fecha);
                            command.Parameters.AddWithValue("@UserID", pedido.UserID);
                            command.ExecuteNonQuery();
                        }

                        // Eliminar items existentes
                        using (var command = new NpgsqlCommand(
                            "DELETE FROM items WHERE idpedidos = @IdPedidos", 
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@IdPedidos", pedido.IdPedidos);
                            command.ExecuteNonQuery();
                        }

                        // Insertar nuevos items si existen
                        if (pedido.items != null && pedido.items.Count > 0)
                        {
                            // Obtener el próximo valor de iddetalle
                            int nextIdDetalle;
                            using (var command = new NpgsqlCommand(
                                "SELECT COALESCE(MAX(iddetalle), 0) + 1 FROM items", 
                                connection, transaction))
                            {
                                nextIdDetalle = Convert.ToInt32(command.ExecuteScalar());
                            }

                            foreach (var item in pedido.items)
                            {
                                // Usar el siguiente valor de iddetalle
                                string insertItemSql = @"
INSERT INTO items (iddetalle, idpedidos, idproducto, cantidad, precio) 
VALUES (@IdDetalle, @IdPedidos, @IdProducto, @Cantidad, @Precio)";

                                using (var command = new NpgsqlCommand(insertItemSql, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@IdDetalle", nextIdDetalle);
                                    command.Parameters.AddWithValue("@IdPedidos", pedido.IdPedidos);
                                    command.Parameters.AddWithValue("@IdProducto", item.IdProducto);
                                    command.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                                    command.Parameters.AddWithValue("@Precio", item.Precio);
                                    
                                    command.ExecuteNonQuery();

                                    // Asignar el iddetalle generado al item
                                    item.IdDetalle = nextIdDetalle;
                                    
                                    // Incrementar para el próximo item
                                    nextIdDetalle++;
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new InvalidOperationException($"Error al actualizar el pedido: {ex.Message}", ex);
                    }
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
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new InvalidOperationException($"Error al eliminar el pedido: {ex.Message}", ex);
                    }
                }
            }
        }
    }
}