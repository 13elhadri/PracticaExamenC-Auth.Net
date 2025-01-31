using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace examn.Database.Entities;
[Table("Funkos")]
public class FunkoEntity
{
    public const long NewId = 0;

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; } = NewId;
    
    public string Nombre { get; set; }
    
    [ForeignKey("Categoria")] 
    [Column("categoria_id")]
    public long CategoriaId { get; set; }
    public CategoriaEntity Categoria { get; set; }
    
    public string Foto { get; set; } = "default.png";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsDeleted { get; set; }
}