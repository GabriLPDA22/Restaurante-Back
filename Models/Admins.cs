using System.Text.Json.Serialization;

namespace CineAPI.Models
{
    public class Admins
    {
        [JsonIgnore]  // Swagger no pedirá `id` en POST
        public int Id { get; set; }

        public string Nombre { get; set; }
        public string Contraseña { get; set; }

        public Admins() { }  // Constructor vacío necesario

        public Admins(string nombre, string contraseña)
        {
            Nombre = nombre;
            Contraseña = contraseña;
        }
    }
}
