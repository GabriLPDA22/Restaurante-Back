using Restaurante.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<IEnumerable<Reservation>> GetByCustomerNameAsync(string customerName);
        Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId);
        Task<Reservation?> GetByIdAsync(int id);
        Task<bool> AddAsync(Reservation reservation);
        Task<bool> UpdateAsync(Reservation reservation);
        Task<bool> DeleteAsync(int id);
    }
}