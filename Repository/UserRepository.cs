using Npgsql;
using CineAPI.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CineAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(Users user)
        {
            if (user.Roles == null || user.Roles.Length == 0)
            {
                user.Roles = new string[] { "User" };
            }

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // Agregamos Telefono y FechaNacimiento
            var query = @"
                INSERT INTO users
                (Nombre, Email, Password, Roles, GoogleId, PictureUrl, Telefono, FechaNacimiento)
                VALUES
                (@Nombre, @Email, @Password, @Roles, @GoogleId, @PictureUrl, @Telefono, @FechaNacimiento)
            ";
            using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("@Nombre", user.Nombre);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.Add("@Password", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)user.Password ?? DBNull.Value;
            command.Parameters.AddWithValue("@Roles", user.Roles);
            command.Parameters.Add("@GoogleId", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)user.GoogleId ?? DBNull.Value;
            command.Parameters.Add("@PictureUrl", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)user.PictureUrl ?? DBNull.Value;
            // Campos nuevos
            command.Parameters.Add("@Telefono", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)user.Telefono ?? DBNull.Value;
            command.Parameters.Add("@FechaNacimiento", NpgsqlTypes.NpgsqlDbType.Date).Value = (object?)user.FechaNacimiento ?? DBNull.Value;

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM users WHERE UserID = @UserID";
            using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserID", id);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            var users = new List<Users>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM users";
            using var command = new NpgsqlCommand(query, connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(new Users
                {
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? null : reader.GetString(reader.GetOrdinal("Password")),
                    Roles = reader.IsDBNull(reader.GetOrdinal("Roles")) ? new string[] { } : reader.GetFieldValue<string[]>(reader.GetOrdinal("Roles")),
                    GoogleId = reader.IsDBNull(reader.GetOrdinal("GoogleId")) ? null : reader.GetString(reader.GetOrdinal("GoogleId")),
                    PictureUrl = reader.IsDBNull(reader.GetOrdinal("PictureUrl")) ? null : reader.GetString(reader.GetOrdinal("PictureUrl")),
                    // Campos nuevos
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                    FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("FechaNacimiento")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaNacimiento"))
                });
            }

            return users;
        }

        public async Task<Users?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM users WHERE UserID = @UserID";
            using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserID", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Users
                {
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? null : reader.GetString(reader.GetOrdinal("Password")),
                    Roles = reader.IsDBNull(reader.GetOrdinal("Roles")) ? new string[] { } : reader.GetFieldValue<string[]>(reader.GetOrdinal("Roles")),
                    GoogleId = reader.IsDBNull(reader.GetOrdinal("GoogleId")) ? null : reader.GetString(reader.GetOrdinal("GoogleId")),
                    PictureUrl = reader.IsDBNull(reader.GetOrdinal("PictureUrl")) ? null : reader.GetString(reader.GetOrdinal("PictureUrl")),
                    // Campos nuevos
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                    FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("FechaNacimiento")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaNacimiento"))
                };
            }

            return null;
        }

        public async Task<Users?> GetByEmailAsync(string email)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM users WHERE Email = @Email";
            using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Users
                {
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? null : reader.GetString(reader.GetOrdinal("Password")),
                    Roles = reader.IsDBNull(reader.GetOrdinal("Roles")) ? new string[] { } : reader.GetFieldValue<string[]>(reader.GetOrdinal("Roles")),
                    GoogleId = reader.IsDBNull(reader.GetOrdinal("GoogleId")) ? null : reader.GetString(reader.GetOrdinal("GoogleId")),
                    PictureUrl = reader.IsDBNull(reader.GetOrdinal("PictureUrl")) ? null : reader.GetString(reader.GetOrdinal("PictureUrl")),
                    // Campos nuevos
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                    FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("FechaNacimiento")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaNacimiento"))
                };
            }

            return null;
        }

        public async Task UpdateAsync(Users user)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // Si estás en desarrollo, imprime información de diagnóstico
            Console.WriteLine($"[DEBUG] Actualizando usuario: UserID={user.UserID}, Nombre={user.Nombre}, Email={user.Email}");
            Console.WriteLine($"[DEBUG] Campos adicionales: Telefono={user.Telefono}, FechaNacimiento={user.FechaNacimiento}");

            // Usamos los nombres exactos de las columnas como están en la base de datos
            var query = @"
        UPDATE users
        SET
            nombre = @Nombre,
            email = @Email,
            password = @Password,
            roles = @Roles,
            googleid = @GoogleId,
            pictureurl = @PictureUrl,
            ""Telefono"" = @Telefono,
            ""FechaNacimiento"" = @FechaNacimiento
        WHERE userid = @UserID
    ";

            using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserID", user.UserID);
            command.Parameters.AddWithValue("@Nombre", user.Nombre);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.Add("@Password", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)user.Password ?? DBNull.Value;
            command.Parameters.AddWithValue("@Roles", user.Roles);
            command.Parameters.Add("@GoogleId", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)user.GoogleId ?? DBNull.Value;
            command.Parameters.Add("@PictureUrl", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)user.PictureUrl ?? DBNull.Value;
            // Campos nuevos con nombres exactos
            command.Parameters.Add("@Telefono", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)user.Telefono ?? DBNull.Value;

            // Manejo especial para la fecha
            if (user.FechaNacimiento.HasValue)
            {
                command.Parameters.Add("@FechaNacimiento", NpgsqlTypes.NpgsqlDbType.Date).Value = user.FechaNacimiento.Value;
                Console.WriteLine($"[DEBUG] Fecha en formato de base de datos: {user.FechaNacimiento.Value:yyyy-MM-dd}");
            }
            else
            {
                command.Parameters.Add("@FechaNacimiento", NpgsqlTypes.NpgsqlDbType.Date).Value = DBNull.Value;
                Console.WriteLine("[DEBUG] Fecha es NULL");
            }

            // Ejecutar la consulta y verificar el número de filas afectadas
            int rowsAffected = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"[DEBUG] Filas actualizadas: {rowsAffected}");

            // Si no se actualizó ninguna fila, podría ser un problema
            if (rowsAffected == 0)
            {
                Console.WriteLine($"[ERROR] No se actualizó ninguna fila para el UserID={user.UserID}");

                // Verificar si el usuario existe
                var checkQuery = "SELECT COUNT(*) FROM users WHERE userid = @UserID";
                using var checkCommand = new NpgsqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@UserID", user.UserID);

                int count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
                if (count == 0)
                {
                    Console.WriteLine($"[ERROR] El usuario con UserID={user.UserID} no existe en la base de datos");
                    throw new Exception($"El usuario con ID {user.UserID} no existe en la base de datos");
                }
                else
                {
                    Console.WriteLine($"[WARN] El usuario existe pero no se actualizó. Posible problema con los valores proporcionados");
                }
            }
        }

        public async Task<Users?> GetByGoogleIdAsync(string googleId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM users WHERE GoogleId = @GoogleId";
            using var command = new NpgsqlCommand(query, connection);

            command.Parameters.Add("@GoogleId", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)googleId ?? DBNull.Value;

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Users
                {
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? null : reader.GetString(reader.GetOrdinal("Password")),
                    Roles = reader.IsDBNull(reader.GetOrdinal("Roles")) ? new string[] { } : reader.GetFieldValue<string[]>(reader.GetOrdinal("Roles")),
                    GoogleId = reader.IsDBNull(reader.GetOrdinal("GoogleId")) ? null : reader.GetString(reader.GetOrdinal("GoogleId")),
                    PictureUrl = reader.IsDBNull(reader.GetOrdinal("PictureUrl")) ? null : reader.GetString(reader.GetOrdinal("PictureUrl")),
                    // Campos nuevos
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                    FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("FechaNacimiento")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaNacimiento"))
                };
            }

            return null;
        }
    }
}