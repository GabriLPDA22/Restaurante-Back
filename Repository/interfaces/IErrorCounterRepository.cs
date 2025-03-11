using Restaurante.Models;
using System.Threading.Tasks;

namespace Restaurante.Repositories.Interfaces
{
    public interface IErrorCounterRepository
    {
        Task<ErrorCounter> GetErrorCounter();
        Task CreateErrorCounter(ErrorCounter errorCounter);
        Task UpdateErrorCounter(ErrorCounter errorCounter);
    }
}