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
			return pedido;
		}

public void Add(Pedido pedido)
{
    // Verificar que el usuario existe usando el nombre correcto de la columna (userid)
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

    // Insertar el pedido
    using (var connection = new NpgsqlConnection(_connectionString))
    {
        connection.Open();
        using (var command = new NpgsqlCommand("INSERT INTO pedidos (fecha, user_id) VALUES (@Fecha, @UserID) RETURNING id_pedidos", connection))
        {
            command.Parameters.AddWithValue("@Fecha", pedido.Fecha);
            command.Parameters.AddWithValue("@UserID", pedido.UserID);
            
            // Recuperar el ID generado
            var id = (int)command.ExecuteScalar();
            pedido.IdPedidos = id;
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
				using (var command = new NpgsqlCommand("DELETE FROM pedidos WHERE id_pedidos = @Id", connection))
				{
					command.Parameters.AddWithValue("@Id", id);
					command.ExecuteNonQuery();
				}
			}
		}
	}
}