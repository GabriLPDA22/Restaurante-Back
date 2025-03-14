using System.Text.Json.Serialization;

namespace CineAPI.Models
{
    public class Admins
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Nombre { get; set; }
        public string Contraseña { get; set; }

        public Admins() { }

        public Admins(string nombre, string contraseña)
        {
            Nombre = nombre;
            Contraseña = contraseña;
        }
    }
}
