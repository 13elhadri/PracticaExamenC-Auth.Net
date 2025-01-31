using System.ComponentModel.DataAnnotations;

namespace examn.Funkos.Dto;

public class FunkoRequestUpdate
{
    [Required]
    public string Foto { get; set; }
}