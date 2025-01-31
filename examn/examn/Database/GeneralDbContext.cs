using examn.Database.Entities;
using examn.User.Models;
using Microsoft.EntityFrameworkCore;

namespace examn.Database;

public class GeneralDbContext: DbContext
{
    public GeneralDbContext(DbContextOptions<GeneralDbContext> options) : base(options) {}

    public DbSet<UserEntity> Usuarios { get; set; }
    public DbSet<CategoriaEntity> Categorias { get; set; }
    public DbSet<FunkoEntity> Funkos { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.Property(e => e.CreatedAt).IsRequired().ValueGeneratedOnAdd();
            entity.Property(e => e.UpdatedAt).IsRequired().ValueGeneratedOnAdd();
            
            entity.HasData(new UserEntity
            {
                Id = 1,
                Username = "pedrito",
                Password = "$2a$12$ATuj2Hpw./1z0jTlWJhnRO2BtV50WycRxH8WdsN3VnCw.5t4Phph6",
                Role = Role.Admin,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            });
            
            entity.HasData(new UserEntity
            {
                Id = 2,
                Username = "anita",
                Password = "$2a$12$Q6XFDZIrRI5O.kxOoCFmIebwZRSmSlRg81el0Sa4WYm5wmhwCgSyq",
                Role = Role.User,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            });
            
            
        });

        modelBuilder.Entity<CategoriaEntity>(entity =>
        {
            entity.Property(e => e.CreatedAt).IsRequired().ValueGeneratedOnAdd();
            entity.Property(e => e.UpdatedAt).IsRequired().ValueGeneratedOnAdd();
            
            modelBuilder.Entity<CategoriaEntity>().HasData(
                new CategoriaEntity
                {
                    Id = 1,
                    Nombre = "Categoría 1",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new CategoriaEntity
                {
                    Id = 2,
                    Nombre = "Categoría 2",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                }
            );
            
        });
        
        modelBuilder.Entity<FunkoEntity>(entity =>
        {
            entity.Property(e => e.CreatedAt).IsRequired().ValueGeneratedOnAdd();
            entity.Property(e => e.UpdatedAt).IsRequired().ValueGeneratedOnAdd();
            
            /*
            // Relación Cliente-Cuenta (1:N)
            entity.HasOne(c => c.Cliente)
                .WithMany(c => c.Cuentas)
                .HasForeignKey(c => c.ClienteId)
                .IsRequired();
            
            // Relación Cuenta-Producto (1:1)
            entity.HasOne(c => c.Producto)
                .WithMany()
                .HasForeignKey(c => c.ProductoId)
                .IsRequired();
            
            // Relación Cuenta-Tarjeta (1:0..1)
            entity.HasOne(c => c.Tarjeta)
                .WithOne()
                .HasForeignKey<CuentaEntity>(c => c.TarjetaId)
            */
            

            entity.HasOne(f => f.Categoria)
                .WithMany()
                .HasForeignKey(f => f.CategoriaId)
                .IsRequired(); 
            
            modelBuilder.Entity<FunkoEntity>().HasData(
                new FunkoEntity
                {
                    Id = 1,
                    Nombre = "Funko Pop - Spider-Man",
                    CategoriaId = 1, 
                    Foto = "spiderman_foto_url",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new FunkoEntity
                {
                    Id = 2,
                    Nombre = "Funko Pop - Batman",
                    CategoriaId = 2, 
                    Foto = "batman_foto_url",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                }
            );
            
        });
    }
    
}