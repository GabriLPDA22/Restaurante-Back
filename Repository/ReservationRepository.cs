using Npgsql;
using Restaurante.Models;
using Restaurante.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly string _connectionString;

        public ReservationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            var reservations = new List<Reservation>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM reservations", connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                reservations.Add(new Reservation
                {
                    Id = reader.GetInt32(0),
                    DateTime = reader.GetDateTime(1),
                    CustomerName = reader.GetString(2),
                    TableId = reader.GetInt32(3)
                });
            }

            return reservations;
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM reservations WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Reservation
                {
                    Id = reader.GetInt32(0),
                    DateTime = reader.GetDateTime(1),
                    CustomerName = reader.GetString(2),
                    TableId = reader.GetInt32(3)
                };
            }

            return null;
        }

        public async Task<bool> AddAsync(Reservation reservation)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                "INSERT INTO reservations (datetime, customername, tableid) VALUES (@DateTime, @CustomerName, @TableId) RETURNING id",
                connection);
            command.Parameters.AddWithValue("@DateTime", reservation.DateTime);
            command.Parameters.AddWithValue("@CustomerName", reservation.CustomerName);
            command.Parameters.AddWithValue("@TableId", reservation.TableId);

            // Obtener el ID generado automáticamente
            var newId = await command.ExecuteScalarAsync();
            if (newId != null)
            {
                reservation.Id = Convert.ToInt32(newId);
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateAsync(Reservation reservation)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                "UPDATE reservations SET datetime = @DateTime, customername = @CustomerName, tableid = @TableId WHERE id = @Id",
                connection);
            command.Parameters.AddWithValue("@DateTime", reservation.DateTime);
            command.Parameters.AddWithValue("@CustomerName", reservation.CustomerName);
            command.Parameters.AddWithValue("@TableId", reservation.TableId);
            command.Parameters.AddWithValue("@Id", reservation.Id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("DELETE FROM reservations WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
}