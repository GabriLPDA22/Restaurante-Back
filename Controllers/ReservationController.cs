using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations() =>
            Ok(await _reservationService.GetAllReservations());

        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id) =>
            await _reservationService.GetReservationById(id) is Reservation reservation ? Ok(reservation) : NotFound();

        [HttpPost]
        public async Task<IActionResult> PostReservation(Reservation reservation) =>
            await _reservationService.CreateReservation(reservation) ? CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation) : BadRequest();

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, Reservation reservation) =>
            id != reservation.Id || !await _reservationService.UpdateReservation(reservation) ? BadRequest() : NoContent();

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id) =>
            await _reservationService.DeleteReservation(id) ? NoContent() : NotFound();
    }
}
