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

        /// Constructor para inyectar el servicio de prueba.
        /// <param name="pruebaService">Servicio de prueba.</param>
        public PruebaController(IPruebaService pruebaService)
        {
            _pruebaService = pruebaService;
        }

        /// 
        /// Obtiene todos los prueba.
        /// <returns>Lista de prueba.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prueba>>> GetAllPrueba()
        {
            var prueba = await _pruebaService.GetAllPrueba();
            return Ok(prueba);
        }

        /// 
        /// Obtiene un prueba por su ID.
        /// 
        /// <param name="id">ID del prueba.</param>
        /// <returns>Prueba.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Prueba>> GetPruebaById(int id)
        {
            var prueba = await _pruebaService.GetPruebaById(id);
            if (prueba == null)
                return NotFound("El prueba no fue encontrado.");
            return Ok(prueba);
        }

        /// 
        /// Crea un nuevo prueba.
        /// 
        /// <param name="prueba">Datos del prueba.</param>
        /// <returns>Prueba creado.</returns>
        [HttpPost]
        public async Task<ActionResult<Prueba>> CreatePrueba(Prueba prueba)
        {
            await _pruebaService.CreatePrueba(prueba);
            return CreatedAtAction(nameof(GetPruebaById), new { id = prueba.ID }, prueba);
        }

        /// 
        /// Actualiza información básica de un prueba existente.
        /// 
        /// <param name="id">ID del prueba.</param>
        /// <param name="pruebaDto">Datos básicos del prueba a actualizar.</param>
        /// <returns>No content si se actualizó correctamente.</returns>
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// 
        /// Elimina un prueba por su ID.
        /// 
        /// <param name="id">ID del prueba a eliminar.</param>
        /// <returns>No content si se eliminó correctamente.</returns>
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