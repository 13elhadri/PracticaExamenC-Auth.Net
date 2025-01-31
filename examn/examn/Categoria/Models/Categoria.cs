namespace examn.Categoria.Models;

public class Categoria
{
    public long Id { get; set; } = 0;
    
    public string Nombre { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; }= DateTime.UtcNow;
    
    public bool IsDeleted { get; set; } = false;
}