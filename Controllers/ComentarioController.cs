using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentarioController : ControllerBase
    {
        private readonly IComentarioService _comentarioService;

        public ComentarioController(IComentarioService comentarioService)
        {
            _comentarioService = comentarioService;
        }

        [HttpGet]
        public async Task<IEnumerable<Comentario>> GetAllComentario() => 
            await _comentarioService.GetAllComentario();

        [HttpGet("{id}")]
        public async Task<ActionResult<Comentario>> GetComentarioById(int id)
        {
            var comentario = await _comentarioService.GetComentarioById(id);
            if (comentario == null)
            {
                return NotFound();
            }

            return comentario;
        }

        [HttpGet("producto/{productoId}")]
        public async Task<IEnumerable<Comentario>> GetComentariosByProducto(int productoId) => 
            await _comentarioService.GetComentariosByProducto(productoId);

        [HttpPost]
        public async Task<ActionResult<Comentario>> CreateComentario(Comentario comentario)
        {
            if (comentario == null)
            {
                return BadRequest();
            }

            await _comentarioService.CreateComentario(comentario);

            return CreatedAtAction(nameof(GetComentarioById), new { id = comentario.IdComentario }, comentario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComentario(int id, Comentario comentario)
        {
            if (id != comentario.IdComentario)
            {
                return BadRequest();
            }

            var existingComentario = await _comentarioService.GetComentarioById(id);
            if (existingComentario == null)
            {
                return NotFound();
            }

            await _comentarioService.UpdateComentario(comentario);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComentario(int id)
        {
            var existingComentario = await _comentarioService.GetComentarioById(id);
            if (existingComentario == null)
            {
                return NotFound();
            }

            await _comentarioService.DeleteComentario(id);

            return NoContent();
        }
    }
}