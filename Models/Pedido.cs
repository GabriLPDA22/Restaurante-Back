public class Pedido
{
    public int IdPedidos { get; set; }
    public DateTime Fecha { get; set; }
    public int UserID { get; set; }
    public List<Items> items { get; set; }
    public Pedido(int idPedidos, DateTime fecha, int userId)
    {
        IdPedidos = idPedidos;
        Fecha = fecha;
        UserID = userId;
        items = new List<Items>();
    }
    public Pedido()
    {
        items = new List<Items>();
    }
}
