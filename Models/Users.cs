public class Users
{
    public int UserID { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? GoogleId { get; set; } // ID único de Google (opcional)
    public string? PictureUrl { get; set; } // Foto de perfil de Google (opcional)
    public string[] Roles { get; set; }

    // Constructor por defecto
    public Users()
    {
        Roles = Array.Empty<string>(); 
    }

    // Constructor opcional para inicialización rápida
    public Users(string nombre, string Email, string password, string? googleId, string? pictureUrl, string[] roles)
    {
        Nombre = nombre;
        Email = Email;
        Password = password;
        GoogleId = googleId;
        PictureUrl = pictureUrl;
        Roles = roles;
    }

    // Método opcional para depuración (ToString)
    public override string ToString()
    {
        return $"{Nombre} ({Email}) - GoogleID: {GoogleId}, Roles: {string.Join(", ", Roles)}";
    }
}
