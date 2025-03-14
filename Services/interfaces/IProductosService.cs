using Restaurante.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Services.Interfaces
{
    public interface IProductosService
    {
        Task AddProductoAsync(Productos producto);
        Task DeleteProductoAsync(int productoID);
        Task<IEnumerable<Productos>> GetAllProductosAsync();
        Task<Productos?> GetProductoByIdAsync(int productoID);
        Task UpdateProductoAsync(Productos producto);
        Task UpdateProductoBasicInfoAsync(int id, ProductoUpdateDto productoDto);
    }
}