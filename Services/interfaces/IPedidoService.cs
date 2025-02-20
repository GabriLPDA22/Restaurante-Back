namespace Restaurante.Services.Interfaces
{
	public interface IPedidoService
	{
		IEnumerable<Pedido> GetAll();
		Pedido GetById(int id);
		void Add(Pedido pedido);
		void Update(Pedido pedido);
		void Delete(int id);
	}
}