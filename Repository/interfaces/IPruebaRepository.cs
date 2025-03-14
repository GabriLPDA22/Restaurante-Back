using Restaurante.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Repositories.Interfaces
{
    public interface IPruebaRepository
    {
        Task<IEnumerable<Prueba>> GetAllAsync();
        Task<Prueba?> GetByIdAsync(int id);
        Task<bool> AddAsync(Prueba prueba);
        Task<bool> UpdateAsync(Prueba prueba);
        Task<bool> DeleteAsync(int id);
    }
}
