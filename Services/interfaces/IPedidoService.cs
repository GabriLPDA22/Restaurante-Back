namespace Restaurante.Services.Interfaces
{
    public interface IPedidoService
    {
        IEnumerable<Pedido> GetAll();
        Pedido GetById(int id);
        // Add the missing method
        IEnumerable<Pedido> GetByUserId(int userId);
        void Add(Pedido pedido);
        void Update(Pedido pedido);
        void Delete(int id);
        object GetItemsByPedidoId(int pedidoId);
    }
}