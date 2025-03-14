using Npgsql;
using Restaurante.Models;
using Restaurante.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Repositories
{
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly string _connectionString;

        public ComentarioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Comentario>> GetAllAsync()
        {
            var comentarios = new List<Comentario>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM comentario", connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32(0);
                string nombre = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty;
                string email = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                string comentarioTexto = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty;
                DateTime fecha = reader.GetDateTime(4);
                int productoId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);

                comentarios.Add(new Comentario(id, nombre, email, comentarioTexto, fecha, productoId));
            }

            return comentarios;
        }

        public async Task<Comentario?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM comentario WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int comentarioId = reader.GetInt32(0);
                string nombre = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty;
                string email = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                string comentarioTexto = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty;
                DateTime fecha = reader.GetDateTime(4);
                int productoId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);

                return new Comentario(comentarioId, nombre, email, comentarioTexto, fecha, productoId);
            }

            return null;
        }

        public async Task<IEnumerable<Comentario>> GetByProductoIdAsync(int productoId)
        {
            var comentarios = new List<Comentario>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                "SELECT * FROM comentario WHERE producto_id = @ProductoId ORDER BY fecha DESC", 
                connection);
            
            command.Parameters.AddWithValue("@ProductoId", productoId);
            
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32(0);
                string nombre = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty;
                string email = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                string comentarioTexto = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty;
                DateTime fecha = reader.GetDateTime(4);
                int prodId = reader.GetInt32(5); // Columna producto_id
                
                comentarios.Add(new Comentario(id, nombre, email, comentarioTexto, fecha, prodId));
            }
            
            return comentarios;
        }

        public async Task<bool> CreateAsync(Comentario comentario)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                "INSERT INTO comentario (nombre, email, comentario_texto, fecha, producto_id) " +
                "VALUES (@Nombre, @Email, @ComentarioTexto, @Fecha, @ProductoId) RETURNING id", 
                connection);
            
            command.Parameters.AddWithValue("@Nombre", comentario.Nombre);
            command.Parameters.AddWithValue("@Email", comentario.Email);
            command.Parameters.AddWithValue("@ComentarioTexto", comentario.ComentarioTexto);
            command.Parameters.AddWithValue("@Fecha", comentario.Fecha);
            command.Parameters.AddWithValue("@ProductoId", comentario.ProductoId);

            var result = await command.ExecuteScalarAsync();
            if (result != null && result != DBNull.Value)
            {
                comentario.IdComentario = Convert.ToInt32(result);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateAsync(Comentario comentario)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                "UPDATE comentario SET nombre = @Nombre, email = @Email, comentario_texto = @ComentarioTexto, " +
                "fecha = @Fecha, producto_id = @ProductoId WHERE id = @Id", 
                connection);
            
            command.Parameters.AddWithValue("@Id", comentario.IdComentario);
            command.Parameters.AddWithValue("@Nombre", comentario.Nombre);
            command.Parameters.AddWithValue("@Email", comentario.Email);
            command.Parameters.AddWithValue("@ComentarioTexto", comentario.ComentarioTexto);
            command.Parameters.AddWithValue("@Fecha", comentario.Fecha);
            command.Parameters.AddWithValue("@ProductoId", comentario.ProductoId);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("DELETE FROM comentario WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}