using Microsoft.AspNetCore.Mvc;
using Restaurante.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Pedido>> GetAll()
        {
            try
            {
                var pedidos = _pedidoService.GetAll();
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al obtener los pedidos: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Pedido> GetById(int id)
        {
            try
            {
                var pedido = _pedidoService.GetById(id);
                if (pedido == null)
                {
                    return NotFound(new { message = $"Pedido con ID {id} no encontrado" });
                }
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al obtener el pedido: {ex.Message}" });
            }
        }

        [HttpGet("usuario/{userId}")]
        public ActionResult<IEnumerable<Pedido>> GetByUserId(int userId)
        {
            try
            {
                var pedidos = _pedidoService.GetByUserId(userId);
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al obtener los pedidos del usuario: {ex.Message}" });
            }
        }

        [HttpGet("{pedidoId}/items")]
        public ActionResult<List<Items>> GetItemsByPedidoId(int pedidoId)
        {
            try
            {
                var items = _pedidoService.GetItemsByPedidoId(pedidoId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al obtener los items del pedido: {ex.Message}" });
            }
        }

        [HttpPost]
        public ActionResult Add([FromBody] PedidoDto pedidoDto)
        {
            try
            {
                var pedido = new Pedido
                {
                    UserID = pedidoDto.UserID,
                    Fecha = DateTime.Now,
                    items = new List<Items>()
                };

                // Convertir los items del DTO al modelo
                foreach (var itemDto in pedidoDto.Items)
                {
                    pedido.items.Add(new Items
                    {
                        IdProducto = itemDto.IdProducto,
                        Cantidad = itemDto.Cantidad,
                        Precio = itemDto.Precio
                    });
                }

                _pedidoService.Add(pedido);
                
                return CreatedAtAction(nameof(GetById), new { id = pedido.IdPedidos }, pedido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al crear el pedido: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Pedido pedido)
        {
            try
            {
                if (id != pedido.IdPedidos)
                {
                    return BadRequest(new { message = "El ID del pedido no coincide" });
                }

                var existingPedido = _pedidoService.GetById(id);
                if (existingPedido == null)
                {
                    return NotFound(new { message = $"Pedido con ID {id} no encontrado" });
                }

                _pedidoService.Update(pedido);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al actualizar el pedido: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var existingPedido = _pedidoService.GetById(id);
                if (existingPedido == null)
                {
                    return NotFound(new { message = $"Pedido con ID {id} no encontrado" });
                }

                _pedidoService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al eliminar el pedido: {ex.Message}" });
            }
        }
    }

    // DTO para la creación de pedidos
    public class PedidoDto
    {
        public int UserID { get; set; }
        public List<ItemDto> Items { get; set; }
    }

    public class ItemDto
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}