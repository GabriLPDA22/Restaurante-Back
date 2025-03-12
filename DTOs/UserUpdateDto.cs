using System;

namespace CineAPI.Models.DTOs
{
    public class UserUpdateDto
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }
}