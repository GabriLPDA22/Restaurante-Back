using Restaurante.Models;
using Restaurante.Repositories.Interfaces;
using Restaurante.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Services
{
    public class PruebaService : IPruebaService
    {
        private readonly IPruebaRepository _pruebaRepository;

        public PruebaService(IPruebaRepository pruebaRepository)
        {
            _pruebaRepository = pruebaRepository;
        }

        public async Task<IEnumerable<Prueba>> GetAllPrueba() => 
            await _pruebaRepository.GetAllAsync();

        public async Task<Prueba?> GetPruebaById(int id) => 
            await _pruebaRepository.GetByIdAsync(id);

        public async Task<bool> CreatePrueba(Prueba prueba) => 
            await _pruebaRepository.AddAsync(prueba);

        public async Task<bool> UpdatePrueba(Prueba prueba) => 
            await _pruebaRepository.UpdateAsync(prueba);

        public async Task<bool> DeletePrueba(int id) => 
            await _pruebaRepository.DeleteAsync(id);
    }
}
