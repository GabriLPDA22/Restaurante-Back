using Restaurante.Models;
using Restaurante.Repositories.Interfaces;
using Restaurante.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationService(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<Reservation>> GetAllReservations() => 
            await _reservationRepository.GetAllAsync();

        public async Task<IEnumerable<Reservation>> GetReservationsByCustomerName(string customerName) =>
            await _reservationRepository.GetByCustomerNameAsync(customerName);

        public async Task<IEnumerable<Reservation>> GetReservationsByUserId(int userId) =>
            await _reservationRepository.GetByUserIdAsync(userId);

        public async Task<Reservation?> GetReservationById(int id) => 
            await _reservationRepository.GetByIdAsync(id);

        public async Task<bool> CreateReservation(Reservation reservation) => 
            await _reservationRepository.AddAsync(reservation);

        public async Task<bool> UpdateReservation(Reservation reservation) => 
            await _reservationRepository.UpdateAsync(reservation);

        public async Task<bool> DeleteReservation(int id) => 
            await _reservationRepository.DeleteAsync(id);
    }
}