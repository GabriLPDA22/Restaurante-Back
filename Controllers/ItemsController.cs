using Microsoft.AspNetCore.Mvc;
using CineAPI.Services.Interfaces;
using Google.Apis.Auth;
using System.Threading.Tasks;
using Restaurante.Services.Interfaces;

namespace Restaurante.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsService _itemsService;

        public ItemsController(IItemsService itemsService)
        {
            _itemsService = itemsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _itemsService.GetAllItemsAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _itemsService.GetItemByIdAsync(id);
            if (item == null)
                return NotFound(new { Message = "Item no encontrado" });

            return Ok(item);
        }


        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] Items item)
        {
            try
            {
                await _itemsService.AddItemAsync(item);
                return CreatedAtAction(nameof(GetItemById), new { id = item.IdDetalle }, item);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] Items item)
        {
            if (id != item.IdDetalle)
                return BadRequest(new { Message = "El ID proporcionado no coincide con el item." });

            try
            {
                await _itemsService.UpdateItemAsync(item);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _itemsService.DeleteItemAsync(id);
            return Ok(new { Message = "Item eliminado correctamente" });
        }

    }

}
