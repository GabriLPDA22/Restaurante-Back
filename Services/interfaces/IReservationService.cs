using Restaurante.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Services.Interfaces
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetAllReservations();
        Task<IEnumerable<Reservation>> GetReservationsByCustomerName(string customerName);
        Task<IEnumerable<Reservation>> GetReservationsByUserId(int userId);
        Task<Reservation?> GetReservationById(int id);
        Task<bool> CreateReservation(Reservation reservation);
        Task<bool> UpdateReservation(Reservation reservation);
        Task<bool> DeleteReservation(int id);
    }
}