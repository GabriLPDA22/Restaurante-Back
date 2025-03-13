using System;
namespace Restaurante.Models{
    public class Prueba
    {
        public int ID { get; set; }
        public int Nombre { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    public Prueba(int ID, string nombre, string Email, string password)
    {   
        ID = ID;
        Nombre = nombre;
        Email = Email;
        Password = password;
    }
}
}