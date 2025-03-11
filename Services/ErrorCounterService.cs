using Restaurante.Repositories.Interfaces;
using System.Threading.Tasks;

namespace Restaurante.Services
{
    public class ErrorCounterService : IErrorCounterService
    {
        private readonly IErrorCounterRepository _errorCounterRepository;

        public ErrorCounterService(IErrorCounterRepository errorCounterRepository)
        {
            _errorCounterRepository = errorCounterRepository;
        }

        public async Task<int> GetErrorCounterValue()
        {
            var errorCounter = await _errorCounterRepository.GetErrorCounter();
            return errorCounter?.valor ?? 0;
        }

        public async Task UpdateErrorCounterValue(int value)
        {
            var errorCounter = await _errorCounterRepository.GetErrorCounter();
            
            if (errorCounter == null)
            {
                errorCounter = new ErrorCounter { valor = value };
                await _errorCounterRepository.CreateErrorCounter(errorCounter);
            }
            else
            {
                errorCounter.valor = value;
                await _errorCounterRepository.UpdateErrorCounter(errorCounter);
            }
        }
    }
}