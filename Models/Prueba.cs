using System;
namespace Restaurante.Models
{
    public class Prueba
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }

        public Prueba(int ID, string nombre, string email, string password)
        {   
            this.ID = ID;
            Nombre = nombre;
            Email = email;
            Password = password;
        }
    }
}