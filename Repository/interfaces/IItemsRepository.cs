namespace Restaurante.Repositories.Interfaces
{
    public interface IItemsRepository
    {
        Task<IEnumerable<Items>> GetAllAsync();
        Task<Items> GetByIdAsync(int id);
        Task AddAsync(Items item);
        Task UpdateAsync(Items item);
        Task DeleteAsync(int id);
    }
}
