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
    public class FavoritosController : ControllerBase
    {
        private readonly IFavoritosService _favoritosService;

        public FavoritosController(IFavoritosService favoritosService)
        {
            _favoritosService = favoritosService;
        }

        /// <summary>
        /// Obtiene todos los favoritos (endpoint administrativo).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Favoritos>>> GetAllFavoritos()
        {
            try
            {
                var favoritos = await _favoritosService.GetAllAsync();
                return Ok(favoritos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al obtener los favoritos: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obtiene los favoritos de un usuario específico.
        /// </summary>
        [HttpGet("usuario/{userId}")]
        public async Task<ActionResult<IEnumerable<Favoritos>>> GetFavoritosByUserId(int userId)
        {
            try
            {
                var favoritos = await _favoritosService.GetByUserIdAsync(userId);
                return Ok(favoritos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al obtener los favoritos del usuario: {ex.Message}" });
            }
        }

        /// <summary>
        /// Verifica si un producto es favorito para un usuario.
        /// </summary>
        [HttpGet("usuario/{userId}/producto/{productoId}")]
        public async Task<ActionResult<bool>> IsProductoFavorito(int userId, int productoId)
        {
            try
            {
                var isFavorito = await _favoritosService.IsProductoFavoritoAsync(userId, productoId);
                return Ok(isFavorito);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al verificar si el producto es favorito: {ex.Message}" });
            }
        }

        /// <summary>
        /// Agrega un producto a los favoritos de un usuario.
        /// </summary>
        [HttpPost("usuario/{userId}/producto/{productoId}")]
        public async Task<ActionResult> AddProductoToFavoritos(int userId, int productoId)
        {
            try
            {
                await _favoritosService.AddProductoToFavoritosAsync(userId, productoId);
                return Ok(new { message = "Producto agregado a favoritos correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al agregar el producto a favoritos: {ex.Message}" });
            }
        }

        /// <summary>
        /// Elimina un producto de los favoritos de un usuario.
        /// </summary>
        [HttpDelete("usuario/{userId}/producto/{productoId}")]
        public async Task<ActionResult> RemoveProductoFromFavoritos(int userId, int productoId)
        {
            try
            {
                await _favoritosService.RemoveProductoFromFavoritosAsync(userId, productoId);
                return Ok(new { message = "Producto eliminado de favoritos correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al eliminar el producto de favoritos: {ex.Message}" });
            }
        }

        /// <summary>
        /// Agrega o elimina un producto de los favoritos de un usuario (toggle).
        /// </summary>
        [HttpPut("usuario/{userId}/producto/{productoId}/toggle")]
        public async Task<ActionResult> ToggleFavorito(int userId, int productoId)
        {
            try
            {
                await _favoritosService.ToggleFavoritoAsync(userId, productoId);
                var isFavorito = await _favoritosService.IsProductoFavoritoAsync(userId, productoId);
                return Ok(new
                {
                    isFavorito = isFavorito,
                    message = isFavorito ? "Producto agregado a favoritos." : "Producto eliminado de favoritos."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al cambiar el estado de favorito: {ex.Message}" });
            }
        }
    }
}