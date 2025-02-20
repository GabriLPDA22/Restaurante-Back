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
				using (var command = new NpgsqlCommand("SELECT * FROM Pedidos", connection))
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var pedido = new Pedido
							{
								IdPedidos = reader.GetInt32(0),
								Fecha = reader.GetDateTime(1),
								Hora = reader.GetTimeSpan(2),
								UserID = reader.GetInt32(3)
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
				using (var command = new NpgsqlCommand("SELECT * FROM Pedidos WHERE IdPedidos = @Id", connection))
				{
					command.Parameters.AddWithValue("@Id", id);
					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							pedido = new Pedido
							{
								IdPedidos = reader.GetInt32(0),
								Fecha = reader.GetDateTime(1),
								Hora = reader.GetTimeSpan(2),
								UserID = reader.GetInt32(3)
							};
						}
					}
				}
			}
			return pedido;
		}

		public void Add(Pedido pedido)
		{
			using (var connection = new NpgsqlConnection(_connectionString))
			{
				connection.Open();
				using (var command = new NpgsqlCommand("INSERT INTO Pedidos (Fecha, Hora, UserID) VALUES (@Fecha, @Hora, @UserID)", connection))
				{
					command.Parameters.AddWithValue("@Fecha", pedido.Fecha);
					command.Parameters.AddWithValue("@Hora", pedido.Hora);
					command.Parameters.AddWithValue("@UserID", pedido.UserID);
					command.ExecuteNonQuery();
				}
			}
		}

		public void Update(Pedido pedido)
		{
			using (var connection = new NpgsqlConnection(_connectionString))
			{
				connection.Open();
				using (var command = new NpgsqlCommand("UPDATE Pedidos SET Fecha = @Fecha, Hora = @Hora, UserID = @UserID WHERE IdPedidos = @Id", connection))
				{
					command.Parameters.AddWithValue("@Id", pedido.IdPedidos);
					command.Parameters.AddWithValue("@Fecha", pedido.Fecha);
					command.Parameters.AddWithValue("@Hora", pedido.Hora);
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
				using (var command = new NpgsqlCommand("DELETE FROM Pedidos WHERE IdPedidos = @Id", connection))
				{
					command.Parameters.AddWithValue("@Id", id);
					command.ExecuteNonQuery();
				}
			}
		}
	}
}


