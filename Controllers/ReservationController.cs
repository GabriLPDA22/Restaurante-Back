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

        [HttpGet("byDate")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservationsByDate([FromQuery] string date)
        {
            try
            {
                // Obtener todas las reservas
                var allReservations = await _reservationService.GetAllReservations();

                // Si no se proporciona una fecha, devolver todas las reservas
                if (string.IsNullOrEmpty(date))
                {
                    return Ok(allReservations);
                }

                // Intentar parsear la fecha recibida
                if (!DateTime.TryParse(date, out DateTime parsedDate))
                {
                    return BadRequest($"Invalid date format: {date}. Expected format: yyyy-MM-dd");
                }

                // Filtrar las reservas por la fecha (solo comparando año, mes y día)
                var filteredReservations = allReservations.Where(r =>
                    r.DateTime.Year == parsedDate.Year &&
                    r.DateTime.Month == parsedDate.Month &&
                    r.DateTime.Day == parsedDate.Day
                );

                // Depuración - imprimir información sobre fechas
                Console.WriteLine($"Requested date: {parsedDate.ToString("yyyy-MM-dd")}");
                foreach (var res in allReservations)
                {
                    Console.WriteLine($"Reservation ID: {res.Id}, Date: {res.DateTime.ToString("yyyy-MM-dd")}, TableId: {res.TableId}");
                }

                return Ok(filteredReservations);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving reservations: {ex.Message}");
            }
        }

    }
}
