using Restaurante.Repositories.Interfaces;
using Restaurante.Services.Interfaces;
using System.Collections.Generic;

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

        public IEnumerable<Pedido> GetByUserId(int userId)
        {
            return _pedidoRepository.GetByUserId(userId);
        }

        public object GetItemsByPedidoId(int pedidoId)
        {
            return _pedidoRepository.GetItemsByPedidoId(pedidoId);
        }

        public void Add(Pedido pedido)
        {
            // Asegúrate de que la fecha se establezca si no está definida
            if (pedido.Fecha == default)
            {
                pedido.Fecha = DateTime.Now;
            }
            
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