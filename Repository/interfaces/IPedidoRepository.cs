using System.Collections.Generic;

namespace Restaurante.Repositories.Interfaces
{
    public interface IPedidoRepository
    {
        IEnumerable<Pedido> GetAll();
        Pedido GetById(int id);
        IEnumerable<Pedido> GetByUserId(int userId);
        List<Items> GetItemsByPedidoId(int pedidoId);
        void Add(Pedido pedido);
        void Update(Pedido pedido);
        void Delete(int id);
    }
}