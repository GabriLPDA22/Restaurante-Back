public class Productos
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; } // New attribute: Descripcion
    public decimal Precio { get; set; }
    public string ImagenUrl { get; set; }
    public List<string> Categorias { get; set; }
    public int Cantidad { get; set; } // New attribute: Cantidad
}
