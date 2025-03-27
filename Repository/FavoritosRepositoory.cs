using Npgsql;
using Restaurante.Models;
using Restaurante.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Repositories
{
    public class FavoritosRepository : IFavoritosRepository
    {
        private readonly string _connectionString;

        public FavoritosRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Favoritos>> GetAllAsync()
        {
            var favoritos = new List<Favoritos>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(@"
                    SELECT f.id, f.user_id, f.producto_id, f.fecha_agregado,
                           p.nombre as nombre_producto, p.descripcion as descripcion_producto, 
                           p.precio as precio_producto, p.imagenurl as imagen_url
                    FROM favoritos f
                    JOIN productos p ON f.producto_id = p.id", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var favorito = new Favoritos
                            {
                                Id = reader.GetInt32(0),
                                UserID = reader.GetInt32(1),
                                ProductoId = reader.GetInt32(2),
                                FechaAgregado = reader.GetDateTime(3),
                                NombreProducto = reader.GetString(4),
                                DescripcionProducto = reader.GetString(5),
                                PrecioProducto = reader.GetDecimal(6),
                                ImagenUrl = reader.GetString(7)
                            };
                            favoritos.Add(favorito);
                        }
                    }
                }
            }
            return favoritos;
        }

        public async Task<IEnumerable<Favoritos>> GetByUserIdAsync(int userId)
        {
            var favoritos = new List<Favoritos>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(@"
                    SELECT f.id, f.user_id, f.producto_id, f.fecha_agregado,
                           p.nombre as nombre_producto, p.descripcion as descripcion_producto, 
                           p.precio as precio_producto, p.imagenurl as imagen_url
                    FROM favoritos f
                    JOIN productos p ON f.producto_id = p.id
                    WHERE f.user_id = @userId", connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var favorito = new Favoritos
                            {
                                Id = reader.GetInt32(0),
                                UserID = reader.GetInt32(1),
                                ProductoId = reader.GetInt32(2),
                                FechaAgregado = reader.GetDateTime(3),
                                NombreProducto = reader.GetString(4),
                                DescripcionProducto = reader.GetString(5),
                                PrecioProducto = reader.GetDecimal(6),
                                ImagenUrl = reader.GetString(7)
                            };
                            favoritos.Add(favorito);
                        }
                    }
                }
            }
            return favoritos;
        }

        public async Task<Favoritos> GetByIdAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(@"
                    SELECT f.id, f.user_id, f.producto_id, f.fecha_agregado,
                           p.nombre as nombre_producto, p.descripcion as descripcion_producto, 
                           p.precio as precio_producto, p.imagenurl as imagen_url
                    FROM favoritos f
                    JOIN productos p ON f.producto_id = p.id
                    WHERE f.id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Favoritos
                            {
                                Id = reader.GetInt32(0),
                                UserID = reader.GetInt32(1),
                                ProductoId = reader.GetInt32(2),
                                FechaAgregado = reader.GetDateTime(3),
                                NombreProducto = reader.GetString(4),
                                DescripcionProducto = reader.GetString(5),
                                PrecioProducto = reader.GetDecimal(6),
                                ImagenUrl = reader.GetString(7)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<Favoritos> GetByUserAndProductIdAsync(int userId, int productoId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(@"
                    SELECT f.id, f.user_id, f.producto_id, f.fecha_agregado,
                           p.nombre as nombre_producto, p.descripcion as descripcion_producto, 
                           p.precio as precio_producto, p.imagenurl as imagen_url
                    FROM favoritos f
                    JOIN productos p ON f.producto_id = p.id
                    WHERE f.user_id = @userId AND f.producto_id = @productoId", connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@productoId", productoId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Favoritos
                            {
                                Id = reader.GetInt32(0),
                                UserID = reader.GetInt32(1),
                                ProductoId = reader.GetInt32(2),
                                FechaAgregado = reader.GetDateTime(3),
                                NombreProducto = reader.GetString(4),
                                DescripcionProducto = reader.GetString(5),
                                PrecioProducto = reader.GetDecimal(6),
                                ImagenUrl = reader.GetString(7)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task AddAsync(Favoritos favorito)
        {
            // Verificar si ya existe este favorito para el usuario
            var existingFavorito = await GetByUserAndProductIdAsync(favorito.UserID, favorito.ProductoId);
            if (existingFavorito != null)
            {
                // Ya existe, no es necesario agregar
                return;
            }

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(
                    "INSERT INTO favoritos (user_id, producto_id, fecha_agregado) VALUES (@userId, @productoId, @fechaAgregado) RETURNING id",
                    connection))
                {
                    command.Parameters.AddWithValue("@userId", favorito.UserID);
                    command.Parameters.AddWithValue("@productoId", favorito.ProductoId);
                    command.Parameters.AddWithValue("@fechaAgregado", favorito.FechaAgregado);
                    favorito.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("DELETE FROM favoritos WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteByUserAndProductIdAsync(int userId, int productoId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(
                    "DELETE FROM favoritos WHERE user_id = @userId AND producto_id = @productoId",
                    connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@productoId", productoId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}