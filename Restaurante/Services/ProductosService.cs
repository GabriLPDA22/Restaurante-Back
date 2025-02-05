using Restaurante.Repositories.Interfaces;
using Restaurante.Services.Interfaces;

namespace Restaurante.Services
{
    public class ProductosService : IProductosService
    {
        private readonly IProductosRepository _productosRepository;

        public ProductosService(IProductosRepository productosRepository)
        {
            _productosRepository = productosRepository ?? throw new ArgumentNullException(nameof(productosRepository));
        }

        public async Task AddProductoAsync(Productos producto)
        {
            // Validaciones o lógica adicional si es necesario
            ValidateProducto(producto);

            await _productosRepository.AddAsync(producto);
        }

        public Task CreatePorductoAsync(Productos porducto)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteProductoAsync(int productoID)
        {
            var producto = await _productosRepository.GetByIdAsync(productoID);
            if (producto == null)
            {
                throw new KeyNotFoundException($"No se encontró el producto con ID {productoID}");
            }

            await _productosRepository.DeleteAsync(productoID);
        }

        public Task<IEnumerable<Productos>> GetAllProductosAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Productos?> GetProductoByIdAsync(int productoID)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePorductoAsync(Productos porducto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProductoAsync(Productos producto)
        {
            throw new NotImplementedException();
        }

        // Método de validación para productos
        private void ValidateProducto(Productos producto)
        {
            if (string.IsNullOrEmpty(producto.Nombre))
            {
                throw new ArgumentException("El nombre del producto no puede estar vacío.");
            }
            // Otras validaciones según sea necesario
        }
    }
}