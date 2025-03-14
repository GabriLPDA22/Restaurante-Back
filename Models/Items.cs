public class Items
{
    public int IdDetalle { get; set; }
    public int IdPedidos { get; set; }
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }

    // Propiedades extendidas para mostrar información del producto
    public string NombreProducto { get; set; }
    public string DescripcionProducto { get; set; }
    public string ImagenUrl { get; set; }

    public Items(int idDetalle, int idPedidos, int idProducto, int cantidad, decimal precio)
    {
        IdDetalle = idDetalle;
        IdPedidos = idPedidos;
        IdProducto = idProducto;
        Cantidad = cantidad;
        Precio = precio;
    }

    public Items() { }
}