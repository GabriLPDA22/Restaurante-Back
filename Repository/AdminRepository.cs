using CineAPI.Models;
using CineAPI.Repositories.Interfaces;
using Npgsql;
using System.Collections.Generic;

namespace CineAPI.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly string _connectionString;

        public AdminRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Admins> GetAll()
        {
            var admins = new List<Admins>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, nombre, contraseña FROM admins", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        admins.Add(new Admins
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Contraseña = reader.GetString(2)
                        });
                    }
                }
            }
            return admins;
        }

        public Admins GetById(int id)  // 🔹 Implementación faltante
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, nombre, contraseña FROM admins WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Admins
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Contraseña = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Add(Admins admin)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "INSERT INTO admins (nombre, contraseña) VALUES (@nombre, @contraseña) RETURNING id", conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", admin.Nombre);
                    cmd.Parameters.AddWithValue("@contraseña", admin.Contraseña);
                    admin.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public bool Update(int id, Admins updatedAdmin)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "UPDATE admins SET nombre = @nombre, contraseña = @contraseña WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@nombre", updatedAdmin.Nombre);
                    cmd.Parameters.AddWithValue("@contraseña", updatedAdmin.Contraseña);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM admins WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
