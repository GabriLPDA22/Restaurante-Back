using Restaurante.Models;
using Restaurante.Repositories.Interfaces;
using Restaurante.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurante.Services
{
    public class ComentarioService : IComentarioService
    {
        private readonly IComentarioRepository _comentarioRepository;

        public ComentarioService(IComentarioRepository comentarioRepository)
        {
            _comentarioRepository = comentarioRepository;
        }

        public async Task<IEnumerable<Comentario>> GetAllComentario() => 
            await _comentarioRepository.GetAllAsync();

        public async Task<Comentario?> GetComentarioById(int id) => 
            await _comentarioRepository.GetByIdAsync(id);

        public async Task<bool> CreateComentario(Comentario comentario) => 
            await _comentarioRepository.CreateAsync(comentario);

        public async Task<bool> UpdateComentario(Comentario comentario) => 
            await _comentarioRepository.UpdateAsync(comentario);
            
        public async Task<bool> DeleteComentario(int id) => 
            await _comentarioRepository.DeleteAsync(id);
    }
}