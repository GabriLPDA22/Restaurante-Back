namespace Restaurante.Repositories.Interfaces
{
    public interface IPedidoRepository
    {
        IEnumerable<Pedido> GetAll();
        Pedido GetById(int id);
        void Add(Pedido pedido);
        void Update(Pedido pedido);
        void Delete(int id);
    }
}