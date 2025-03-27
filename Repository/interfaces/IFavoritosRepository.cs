using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurante.Models;

namespace Restaurante.Repositories.Interfaces
{
    public interface IFavoritosRepository
    {
        Task<IEnumerable<Favoritos>> GetAllAsync();
        Task<IEnumerable<Favoritos>> GetByUserIdAsync(int userId);
        Task<Favoritos> GetByIdAsync(int id);
        Task<Favoritos> GetByUserAndProductIdAsync(int userId, int productoId);
        Task AddAsync(Favoritos favorito);
        Task DeleteAsync(int id);
        Task DeleteByUserAndProductIdAsync(int userId, int productoId);
    }
}