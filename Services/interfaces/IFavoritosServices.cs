using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurante.Models;

namespace Restaurante.Services.Interfaces
{
    public interface IFavoritosService
    {
        Task<IEnumerable<Favoritos>> GetAllAsync();
        Task<IEnumerable<Favoritos>> GetByUserIdAsync(int userId);
        Task<Favoritos> GetByIdAsync(int id);
        Task<bool> IsProductoFavoritoAsync(int userId, int productoId);
        Task AddAsync(Favoritos favorito);
        Task AddProductoToFavoritosAsync(int userId, int productoId);
        Task DeleteAsync(int id);
        Task RemoveProductoFromFavoritosAsync(int userId, int productoId);
        Task ToggleFavoritoAsync(int userId, int productoId);
    }
}