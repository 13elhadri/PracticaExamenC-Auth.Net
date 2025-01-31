using examn.Categoria.Mappers;
using examn.Database.Entities;
using examn.Funkos.Models;

namespace examn.Funkos.Mapper;

public static class FunkoMapper
{
    
    public static Funko ToModelFromEntity(this FunkoEntity funkoEntity)
    {
        return new Funko
        {
            Id = funkoEntity.Id,
            Nombre = funkoEntity.Nombre,
            Categoria = funkoEntity.Categoria.ToModelFromEntity(),
            Foto = funkoEntity.Foto,
            CreatedAt = funkoEntity.CreatedAt,
            UpdatedAt = funkoEntity.UpdatedAt,
            IsDeleted = funkoEntity.IsDeleted
        };
    }
    
    public static FunkoEntity ToEntityFromModel(this Funko funkoEntity)
    {
        return new FunkoEntity
        {
            Nombre = funkoEntity.Nombre,
            CategoriaId = funkoEntity.Categoria.Id,
            Foto = funkoEntity.Foto,
            CreatedAt = funkoEntity.CreatedAt,
            UpdatedAt = funkoEntity.UpdatedAt,
            IsDeleted = funkoEntity.IsDeleted
        };
    }
    
}