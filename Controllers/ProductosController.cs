using Microsoft.AspNetCore.Mvc;
using Restaurante.Models.DTOs;
using Restaurante.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductosService _productosService;

        /// <summary>
        /// Constructor para inyectar el servicio de productos.
        /// </summary>
        /// <param name="productosService">Servicio de productos.</param>
        public ProductosController(IProductosService productosService)
        {
            _productosService = productosService;
        }

        /// <summary>
        /// Obtiene todos los productos.
        /// </summary>
        /// <returns>Lista de productos.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Productos>>> GetAllProductos()
        {
            var productos = await _productosService.GetAllProductosAsync();
            return Ok(productos);
        }

        /// <summary>
        /// Obtiene un producto por su ID.
        /// </summary>
        /// <param name="id">ID del producto.</param>
        /// <returns>Producto.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Productos>> GetProductoById(int id)
        {
            var producto = await _productosService.GetProductoByIdAsync(id);
            if (producto == null)
                return NotFound("El producto no fue encontrado.");
            return Ok(producto);
        }

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        /// <param name="producto">Datos del producto.</param>
        /// <returns>Producto creado.</returns>
        [HttpPost]
        public async Task<ActionResult<Productos>> CreateProducto(Productos producto)
        {
            await _productosService.AddProductoAsync(producto);
            return CreatedAtAction(nameof(GetProductoById), new { id = producto.Id }, producto);
        }

        /// <summary>
        /// Actualiza información básica de un producto existente.
        /// </summary>
        /// <param name="id">ID del producto.</param>
        /// <param name="productoDto">Datos básicos del producto a actualizar.</param>
        /// <returns>No content si se actualizó correctamente.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProducto(int id, ProductoUpdateDto productoDto)
        {
            try
            {
                await _productosService.UpdateProductoBasicInfoAsync(id, productoDto);
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

        /// <summary>
        /// Elimina un producto por su ID.
        /// </summary>
        /// <param name="id">ID del producto a eliminar.</param>
        /// <returns>No content si se eliminó correctamente.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProducto(int id)
        {
            var producto = await _productosService.GetProductoByIdAsync(id);
            if (producto == null)
                return NotFound("El producto no fue encontrado.");

            await _productosService.DeleteProductoAsync(id);
            return NoContent();
        }
    }
}