using System;

namespace Restaurante.Models
{
    public class Comentario
    {
        public int IdComentario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ComentarioTexto { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }

        public Comentario(int idComentario, string nombre, string email, string comentarioTexto, DateTime fecha)
        {
            IdComentario = idComentario;
            Nombre = nombre;
            Email = email;
            ComentarioTexto = comentarioTexto;
            Fecha = fecha;
        }

        public Comentario() 
        {
            Fecha = DateTime.Now;
        }
    }
}