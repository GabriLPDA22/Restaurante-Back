using Restaurante.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Services.Interfaces
{
    public interface IComentarioService
    {
        Task<IEnumerable<Comentario>> GetAllComentario();
        Task<Comentario?> GetComentarioById(int id);
        Task<IEnumerable<Comentario>> GetComentariosByProducto(int productoId);
        Task<bool> CreateComentario(Comentario comentario);
        Task<bool> UpdateComentario(Comentario comentario);
        Task<bool> DeleteComentario(int id);
    }
}