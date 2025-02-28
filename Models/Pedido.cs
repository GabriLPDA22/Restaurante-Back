public class Pedido
{
    public int IdPedidos { get; set; }
    public DateTime Fecha { get; set; }
    public int UserID { get; set; }
    public List<Items> Itmes { get; set; }
    public Pedido(int idPedidos, DateTime fecha, int userId)
    {
        IdPedidos = idPedidos;
        Fecha = fecha;
        UserID = userId;
        Itmes = new List<Items>();
    }
    public Pedido()
    {
        Itmes = new List<Items>();
    }
}
