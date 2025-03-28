using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurante.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly IReservationService _reservationService;
        private readonly IProductosService _productosService;

        public HistorialController(
            IPedidoService pedidoService,
            IReservationService reservationService,
            IProductosService productosService)
        {
            _pedidoService = pedidoService;
            _reservationService = reservationService;
            _productosService = productosService;
        }

        [HttpGet("pedidos/{userId}")]
        public ActionResult<List<PedidoHistorialDto>> GetHistorialPedidos(int userId)
        {
            try
            {
                var pedidos = _pedidoService.GetByUserId(userId).ToList();
                if (pedidos == null || !pedidos.Any())
                {
                    return Ok(new List<PedidoHistorialDto>()); 
                }

                var historialPedidos = new List<PedidoHistorialDto>();

                foreach (var pedido in pedidos)
                {
                    var itemsDetallados = new List<ItemDetalladoDto>();
                    decimal total = 0;

                    foreach (var item in pedido.items)
                    {
                        var producto = _productosService.GetProductoByIdAsync(item.IdProducto).Result;
                        var itemDetallado = new ItemDetalladoDto
                        {
                            Nombre = producto?.Nombre ?? "Producto no disponible",
                            Cantidad = item.Cantidad,
                            PrecioUnitario = item.Precio,
                            Subtotal = item.Cantidad * item.Precio
                        };
                        
                        itemsDetallados.Add(itemDetallado);
                        total += itemDetallado.Subtotal;
                    }

                    var pedidoDto = new PedidoHistorialDto
                    {
                        IdPedido = pedido.IdPedidos,
                        Fecha = pedido.Fecha,
                        Items = itemsDetallados,
                        TotalPedido = total
                    };

                    historialPedidos.Add(pedidoDto);
                }

                return Ok(historialPedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error al obtener el historial de pedidos: {ex.Message}" });
            }
        }

        [HttpGet("reservas/usuario/{userId}")]
        public async Task<ActionResult<List<ReservaHistorialDto>>> GetHistorialReservasByUserId(int userId)
        {
            try
            {
                var todasLasReservas = await _reservationService.GetAllReservations();
                var reservasUsuario = todasLasReservas
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.DateTime)
                    .ToList();

                if (!reservasUsuario.Any())
                {
                    return Ok(new List<ReservaHistorialDto>()); 
                }

                var historialReservas = reservasUsuario.Select(r => new ReservaHistorialDto
                {
                    Id = r.Id,
                    Fecha = r.DateTime,
                    NumeroMesa = r.TableId
                }).ToList();

                return Ok(historialReservas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error al obtener el historial de reservas: {ex.Message}" });
            }
        }
        [HttpGet("reservas/{customerName}")]
        [Obsolete("Este método será eliminado en futuras versiones. Usar GET reservas/usuario/{userId} en su lugar.")]
        public async Task<ActionResult<List<ReservaHistorialDto>>> GetHistorialReservas(string customerName)
        {
            try
            {
                var todasLasReservas = await _reservationService.GetAllReservations();
                var reservasUsuario = todasLasReservas
                    .Where(r => r.CustomerName.Equals(customerName, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(r => r.DateTime)
                    .ToList();

                if (!reservasUsuario.Any())
                {
                    return Ok(new List<ReservaHistorialDto>()); // Devuelve una lista vacía si no hay reservas
                }

                var historialReservas = reservasUsuario.Select(r => new ReservaHistorialDto
                {
                    Id = r.Id,
                    Fecha = r.DateTime,
                    NumeroMesa = r.TableId
                }).ToList();

                return Ok(historialReservas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error al obtener el historial de reservas: {ex.Message}" });
            }
        }
    }

    // DTOs 
    public class PedidoHistorialDto
    {
        public int IdPedido { get; set; }
        public DateTime Fecha { get; set; }
        public List<ItemDetalladoDto> Items { get; set; }
        public decimal TotalPedido { get; set; }
    }

    public class ItemDetalladoDto
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class ReservaHistorialDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int NumeroMesa { get; set; }
    }
}