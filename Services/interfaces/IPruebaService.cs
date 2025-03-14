using Restaurante.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Services.Interfaces
{
    public interface IPruebaService
    {
        Task<IEnumerable<Prueba>> GetAllPrueba();
        Task<Prueba?> GetPruebaById(int ID);
        Task<bool> CreatePrueba(Prueba prueba);
        Task<bool> UpdatePrueba(Prueba prueba);
        Task<bool> DeletePrueba(int ID);
    }
}
