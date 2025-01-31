using examn.Database.Entities;

namespace examn.Categoria.Mappers;

public static class CategoriaMapper
{
    public static Models.Categoria ToModelFromEntity(this CategoriaEntity entity)
    {
        return new Models.Categoria()
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsDeleted = entity.IsDeleted
        };
    }
}