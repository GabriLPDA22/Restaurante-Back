using Npgsql;
using Restaurante.Models;
using Restaurante.Repositories.Interfaces;
using System;
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
            var pruebas = new List<Prueba>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM prueba", connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32(0);
                string nombre = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty;
                string email = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                string password = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty;
                
                pruebas.Add(new Prueba(id, nombre, email, password));
            }

            return pruebas;
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
                int pruebaId = reader.GetInt32(0);
                string nombre = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty;
                string email = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                string password = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty;
                
                return new Prueba(pruebaId, nombre, email, password);
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

            // Manejar correctamente los valores nulos
            if (prueba.Nombre != null)
                command.Parameters.AddWithValue("@Nombre", prueba.Nombre);
            else
                command.Parameters.AddWithValue("@Nombre", DBNull.Value);

            if (prueba.Email != null)
                command.Parameters.AddWithValue("@Email", prueba.Email);
            else
                command.Parameters.AddWithValue("@Email", DBNull.Value);

            if (prueba.Password != null)
                command.Parameters.AddWithValue("@Password", prueba.Password);
            else
                command.Parameters.AddWithValue("@Password", DBNull.Value);

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
                
            command.Parameters.AddWithValue("@Id", prueba.ID);
            
            // Manejar correctamente los valores nulos
            if (prueba.Nombre != null)
                command.Parameters.AddWithValue("@Nombre", prueba.Nombre);
            else
                command.Parameters.AddWithValue("@Nombre", DBNull.Value);

            if (prueba.Email != null)
                command.Parameters.AddWithValue("@Email", prueba.Email);
            else
                command.Parameters.AddWithValue("@Email", DBNull.Value);

            if (prueba.Password != null)
                command.Parameters.AddWithValue("@Password", prueba.Password);
            else
                command.Parameters.AddWithValue("@Password", DBNull.Value);

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