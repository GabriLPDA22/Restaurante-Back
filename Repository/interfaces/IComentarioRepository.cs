using Restaurante.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Repositories.Interfaces
{
    public interface IComentarioRepository
    {
        Task<IEnumerable<Comentario>> GetAllAsync();
        Task<Comentario?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Comentario comentario);
        Task<bool> UpdateAsync(Comentario comentario);
        Task<bool> DeleteAsync(int id);
    }
}