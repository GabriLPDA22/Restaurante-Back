using Npgsql;
using Restaurante.Models;
using Restaurante.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Repositories
{
    public class PruebaRepository : IPruebaRepository
    {
        private readonly string _connectionString;

        public PruebaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Prueba>> GetAllAsync()
        {
            var prueba = new List<Prueba>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM prueba", connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                prueba.Add(new Prueba
                {   
                    ID = reader.GetInt32(0),
                    Nombre = reader.GetInt32(1),
                    Email = reader.GetDateTime(2),
                    Password = reader.GetInt32(3)
                });
            }

            return prueba;
        }

        public async Task<Prueba?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM prueba WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
            return new Prueba
            {
                ID = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3)
            };
            }

            return null;
        }
        
        public async Task<bool> AddAsync(Prueba prueba)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
            "INSERT INTO prueba (nombre, email, password) VALUES (@Nombre, @Email, @Password) RETURNING id",
            connection);
            command.Parameters.AddWithValue("@Nombre", prueba.Nombre);
            command.Parameters.AddWithValue("@Email", prueba.Email);
            command.Parameters.AddWithValue("@Password", prueba.Password);

            // Obtener el ID generado automáticamente
            var newId = await command.ExecuteScalarAsync();
            if (newId != null)
            {
            prueba.ID = Convert.ToInt32(newId);
            return true;
            }

            return false;
        }

        public async Task<bool> UpdateAsync(Prueba prueba)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                "UPDATE prueba SET nombre = @Nombre, email = @Email, password = @Password WHERE id = @Id",
                connection);
            command.Parameters.AddWithValue("@Nombre", prueba.Nombre);
            command.Parameters.AddWithValue("@Email", prueba.Email);
            command.Parameters.AddWithValue("@Password", prueba.Password);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("DELETE FROM prueba WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
}