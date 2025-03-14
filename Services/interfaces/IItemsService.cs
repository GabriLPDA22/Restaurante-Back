namespace Restaurante.Services.Interfaces
{
    public interface IItemsService
    {
        Task<IEnumerable<Items>> GetAllItemsAsync();
        Task<Items> GetItemByIdAsync(int id);
        Task AddItemAsync(Items item);
        Task UpdateItemAsync(Items item);
        Task DeleteItemAsync(int id);
    }
}
