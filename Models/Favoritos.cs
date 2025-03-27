namespace Restaurante.Models;
public class Favoritos
{
    public int Id { get; set; }
    public int UserID { get; set; }
    public int ProductoId { get; set; }
    public DateTime FechaAgregado { get; set; }
    public string? NombreProducto { get; set; }
    public string? DescripcionProducto { get; set; }
    public decimal? PrecioProducto { get; set; }
    public string? ImagenUrl { get; set; }

    public Favoritos()
    {
        FechaAgregado = DateTime.Now;
    }

    public Favoritos(int id, int userId, int productoId)
    {
        Id = id;
        UserID = userId;
        ProductoId = productoId;
        FechaAgregado = DateTime.Now;
    }
}