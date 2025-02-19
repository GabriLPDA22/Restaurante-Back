using Microsoft.AspNetCore.Mvc;
using Restaurante.Services.Interfaces;
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
			var pedidos = _pedidoService.GetAll();
			return Ok(pedidos);
		}

		[HttpGet("{id}")]
		public ActionResult<Pedido> GetById(int id)
		{
			var pedido = _pedidoService.GetById(id);
			if (pedido == null)
			{
				return NotFound();
			}
			return Ok(pedido);
		}

		[HttpPost]
		public ActionResult Add([FromBody] Pedido pedido)
		{
			_pedidoService.Add(pedido);
			return CreatedAtAction(nameof(GetById), new { id = pedido.IdPedidos }, pedido);
		}

		[HttpPut("{id}")]
		public ActionResult Update(int id, [FromBody] Pedido pedido)
		{
			if (id != pedido.IdPedidos)
			{
				return BadRequest();
			}

			_pedidoService.Update(pedido);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public ActionResult Delete(int id)
		{
			var existingPedido = _pedidoService.GetById(id);
			if (existingPedido == null)
			{
				return NotFound();
			}

			_pedidoService.Delete(id);
			return NoContent();
		}
	}
}