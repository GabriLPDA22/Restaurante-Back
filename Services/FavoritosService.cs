using Restaurante.Models;
using Restaurante.Repositories.Interfaces;
using Restaurante.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Services
{
    public class FavoritosService : IFavoritosService
    {
        private readonly IFavoritosRepository _favoritosRepository;
        private readonly IProductosRepository _productosRepository;

        public FavoritosService(IFavoritosRepository favoritosRepository, IProductosRepository productosRepository)
        {
            _favoritosRepository = favoritosRepository;
            _productosRepository = productosRepository;
        }

        public async Task<IEnumerable<Favoritos>> GetAllAsync()
        {
            return await _favoritosRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Favoritos>> GetByUserIdAsync(int userId)
        {
            return await _favoritosRepository.GetByUserIdAsync(userId);
        }

        public async Task<Favoritos> GetByIdAsync(int id)
        {
            return await _favoritosRepository.GetByIdAsync(id);
        }

        public async Task<bool> IsProductoFavoritoAsync(int userId, int productoId)
        {
            var favorito = await _favoritosRepository.GetByUserAndProductIdAsync(userId, productoId);
            return favorito != null;
        }

        public async Task AddAsync(Favoritos favorito)
        {
            // Validar que existe el producto
            var producto = await _productosRepository.GetByIdAsync(favorito.ProductoId);
            if (producto == null)
            {
                throw new ArgumentException($"El producto con ID {favorito.ProductoId} no existe.");
            }

            await _favoritosRepository.AddAsync(favorito);
        }

        public async Task AddProductoToFavoritosAsync(int userId, int productoId)
        {
            // Validar que existe el producto
            var producto = await _productosRepository.GetByIdAsync(productoId);
            if (producto == null)
            {
                throw new ArgumentException($"El producto con ID {productoId} no existe.");
            }

            var favorito = new Favoritos
            {
                UserID = userId,
                ProductoId = productoId,
                FechaAgregado = DateTime.Now
            };

            await _favoritosRepository.AddAsync(favorito);
        }

        public async Task DeleteAsync(int id)
        {
            await _favoritosRepository.DeleteAsync(id);
        }

        public async Task RemoveProductoFromFavoritosAsync(int userId, int productoId)
        {
            await _favoritosRepository.DeleteByUserAndProductIdAsync(userId, productoId);
        }

        public async Task ToggleFavoritoAsync(int userId, int productoId)
        {
            var isFavorito = await IsProductoFavoritoAsync(userId, productoId);
            
            if (isFavorito)
            {
                await RemoveProductoFromFavoritosAsync(userId, productoId);
            }
            else
            {
                await AddProductoToFavoritosAsync(userId, productoId);
            }
        }
    }
}