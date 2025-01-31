namespace examn.Funkos.Models;

public class Funko
{
    public long Id { get; set; } = 0;
    
    public string Nombre { get; set; }
    
    public Categoria.Models.Categoria Categoria { get; set; }

    public string Foto { get; set; } = "default.png";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
}