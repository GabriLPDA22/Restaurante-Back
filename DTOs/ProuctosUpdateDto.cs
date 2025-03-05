namespace Restaurante.Models.DTOs
{
    public class ProductoUpdateDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public List<string> Categorias { get; set; }
    }
}