public class Pedido
{
    public int IdPedidos { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public int UserID { get; set; }
    public List<Detalle> Detalles { get; set; }
    public Pedido(int idPedidos, DateTime fecha, TimeSpan hora, int userId)
    {
        IdPedidos = idPedidos;
        Fecha = fecha;
        Hora = hora;
        UserID = userId;
        Detalles = new List<Detalle>();
    }
    public Pedido()
    {
        Detalles = new List<Detalle>();
    }
}
