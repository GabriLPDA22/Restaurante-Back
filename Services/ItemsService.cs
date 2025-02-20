using Restaurante.Repositories.Interfaces;
using Restaurante.Services.Interfaces;


public class ItemsService : IItemsService
{
    private readonly IItemsRepository _itemsRepository;

    public ItemsService(IItemsRepository itemsRepository)
    {
        _itemsRepository = itemsRepository;
    }

    public async Task<IEnumerable<Items>> GetAllItemsAsync()
    {
        return await _itemsRepository.GetAllAsync();
    }

    public async Task<Items> GetItemByIdAsync(int id)
    {
        return await _itemsRepository.GetByIdAsync(id);
    }

    public async Task AddItemAsync(Items item)
    {
        await _itemsRepository.AddAsync(item);
    }

    public async Task UpdateItemAsync(Items item)
    {
        await _itemsRepository.UpdateAsync(item);
    }

    public async Task DeleteItemAsync(int id)
    {
        await _itemsRepository.DeleteAsync(id);
    }
}
