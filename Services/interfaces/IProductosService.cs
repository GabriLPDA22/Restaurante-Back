namespace Restaurante.Services.Interfaces
{
    public interface IProductosService
    {
        Task AddProductoAsync(Productos producto);
        Task DeleteProductoAsync(int productoID);
        Task<IEnumerable<Productos>> GetAllProductosAsync();
        Task<Productos?> GetProductoByIdAsync(int productoID);
        Task UpdateProductoAsync(Productos producto);
    }
}
