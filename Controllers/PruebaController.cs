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
    public class PruebaController : ControllerBase
    {
        private readonly IPruebaService _pruebaService;

        public PruebaController(IPruebaService pruebaService)
        {
            _pruebaService = pruebaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prueba>>> GetAllPrueba()
        {
            var prueba = await _pruebaService.GetAllPrueba();
            return Ok(prueba);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Prueba>> GetPruebaById(int id)
        {
            var prueba = await _pruebaService.GetPruebaById(id);
            if (prueba == null)
                return NotFound("El prueba no fue encontrado.");
            return Ok(prueba);
        }

        [HttpPost]
        public async Task<ActionResult<Prueba>> CreatePrueba(Prueba prueba)
        {
            // Asegurarnos de que no se requiera Email para la creación
            var success = await _pruebaService.CreatePrueba(prueba);
            
            if (!success)
                return BadRequest("No se pudo crear el registro.");
                
            return CreatedAtAction(nameof(GetPruebaById), new { id = prueba.ID }, prueba);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePrueba(int id, Prueba prueba)
        {
            try
            {
                if (id != prueba.ID)
                    return BadRequest("ID de prueba no coincide.");
                
                var success = await _pruebaService.UpdatePrueba(prueba);
                if (!success)
                    return NotFound("El prueba no fue encontrado.");
                    
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePrueba(int id)
        {
            var prueba = await _pruebaService.GetPruebaById(id);
            if (prueba == null)
                return NotFound("El prueba no fue encontrado.");

            await _pruebaService.DeletePrueba(id);
            return NoContent();
        }
    }
}