using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Repositories.Interfaces
{
    public interface IProductosRepository
    {
        Task AddAsync(Productos producto);
        Task DeleteAsync(int productoID);
        Task<IEnumerable<Productos>> GetAllAsync();
        Task<Productos?> GetByIdAsync(int productoID);
        Task UpdateAsync(Productos producto);
    }
}