using Npgsql;
using Restaurante.Repositories.Interfaces;
using Restaurante.Models;
using System;
using System.Threading.Tasks;

namespace Restaurante.Repositories
{
    public class ErrorCounterRepository : IErrorCounterRepository
    {
        private readonly string _connectionString;

        public ErrorCounterRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<ErrorCounter> GetErrorCounter()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM errorcounters LIMIT 1";
            using var command = new NpgsqlCommand(query, connection);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ErrorCounter
                {
                    valor = reader.GetInt32(reader.GetOrdinal("valor"))
                };
            }

            return null;
        }

        public async Task CreateErrorCounter(ErrorCounter errorCounter)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO errorcounters
                (valor)
                VALUES
                (@valor)
            ";
            using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("@valor", errorCounter.valor);
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateErrorCounter(ErrorCounter errorCounter)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // Si estás en desarrollo, imprime información de diagnóstico
            Console.WriteLine($"[DEBUG] Actualizando contador de errores: valor={errorCounter.valor}");

            var query = @"
                UPDATE errorcounters
                SET valor = @valor
                WHERE id = (SELECT id FROM errorcounters LIMIT 1)
            ";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@valor", errorCounter.valor);

            // Ejecutar la consulta y verificar el número de filas afectadas
            int rowsAffected = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"[DEBUG] Filas actualizadas: {rowsAffected}");

            // Si no se actualizó ninguna fila, podría ser un problema
            if (rowsAffected == 0)
            {
                Console.WriteLine("[ERROR] No se actualizó ninguna fila para el contador de errores");

                // Verificar si existe algún registro de contador
                var checkQuery = "SELECT COUNT(*) FROM errorcounters";
                using var checkCommand = new NpgsqlCommand(checkQuery, connection);

                int count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
                if (count == 0)
                {
                    Console.WriteLine("[INFO] No existe un contador de errores, se creará uno nuevo");
                    await CreateErrorCounter(errorCounter);
                }
                else
                {
                    Console.WriteLine("[WARN] El contador existe pero no se actualizó. Posible problema con los valores proporcionados");
                }
            }
        }
    }
}