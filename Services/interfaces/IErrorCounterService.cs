using System.Threading.Tasks;

namespace Restaurante.Services
{
    public interface IErrorCounterService
    {
        Task<int> GetErrorCounterValue();
        Task UpdateErrorCounterValue(int value);
    }
}