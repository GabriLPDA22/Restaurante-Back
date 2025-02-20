using Restaurante.Repositories.Interfaces;
using Restaurante.Services.Interfaces;

namespace Restaurante.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoService(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public IEnumerable<Pedido> GetAll()
        {
            return _pedidoRepository.GetAll();
        }

        public Pedido GetById(int id)
        {
            return _pedidoRepository.GetById(id);
        }

        public void Add(Pedido pedido)
        {
            _pedidoRepository.Add(pedido);
        }

        public void Update(Pedido pedido)
        {
            _pedidoRepository.Update(pedido);
        }

        public void Delete(int id)
        {
            _pedidoRepository.Delete(id);
        }
    }
}