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
        public int ProductoId { get; set; }

        public Comentario(int idComentario, string nombre, string email, string comentarioTexto, DateTime fecha, int productoId)
        {
            IdComentario = idComentario;
            Nombre = nombre;
            Email = email;
            ComentarioTexto = comentarioTexto;
            Fecha = fecha;
            ProductoId = productoId;
        }

        public Comentario() 
        {
            Nombre = string.Empty;
            Email = string.Empty;
            ComentarioTexto = string.Empty;
            Fecha = DateTime.Now;
        }
    }
}