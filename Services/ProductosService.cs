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

        public async Task<IEnumerable<Productos>> GetAllProductosAsync()
        {
            // Implementación del método para obtener todos los productos
            return await _productosRepository.GetAllAsync();
        }

        public async Task<Productos?> GetProductoByIdAsync(int productoID)
        {
            // Implementación del método para obtener un producto por ID
            return await _productosRepository.GetByIdAsync(productoID);
        }

        public async Task UpdateProductoAsync(Productos producto)
        {
            // Validaciones antes de actualizar
            ValidateProducto(producto);
            
            var existingProducto = await _productosRepository.GetByIdAsync(producto.Id);
            if (existingProducto == null)
            {
                throw new KeyNotFoundException($"No se encontró el producto con ID {producto.Id}");
            }

            await _productosRepository.UpdateAsync(producto);
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

        // Este método parece estar duplicado y mal escrito (Porducto en lugar de Producto)
        // Se puede eliminar o corregir según sea necesario
        public Task CreatePorductoAsync(Productos porducto)
        {
            // Redirigimos al método correcto
            return AddProductoAsync(porducto);
        }

        // Este método también parece estar duplicado y mal escrito
        // Se puede eliminar o corregir según sea necesario
        public Task UpdatePorductoAsync(Productos porducto)
        {
            // Redirigimos al método correcto
            return UpdateProductoAsync(porducto);
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