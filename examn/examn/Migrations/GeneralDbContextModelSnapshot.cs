﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using examn.Database;

#nullable disable

namespace examn.Migrations
{
    [DbContext(typeof(GeneralDbContext))]
    partial class GeneralDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("examn.Database.Entities.CategoriaEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Categorias");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            CreatedAt = new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2914),
                            IsDeleted = false,
                            Nombre = "Categoría 1",
                            UpdatedAt = new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2914)
                        },
                        new
                        {
                            Id = 2L,
                            CreatedAt = new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2916),
                            IsDeleted = false,
                            Nombre = "Categoría 2",
                            UpdatedAt = new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2916)
                        });
                });

            modelBuilder.Entity("examn.Database.Entities.FunkoEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("CategoriaId")
                        .HasColumnType("bigint")
                        .HasColumnName("categoria_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Foto")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CategoriaId");

                    b.ToTable("Funkos");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            CategoriaId = 1L,
                            CreatedAt = new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(3881),
                            Foto = "spiderman_foto_url",
                            IsDeleted = false,
                            Nombre = "Funko Pop - Spider-Man",
                            UpdatedAt = new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(3882)
                        },
                        new
                        {
                            Id = 2L,
                            CategoriaId = 2L,
                            CreatedAt = new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(3883),
                            Foto = "batman_foto_url",
                            IsDeleted = false,
                            Nombre = "Funko Pop - Batman",
                            UpdatedAt = new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(3884)
                        });
                });

            modelBuilder.Entity("examn.Database.Entities.UserEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Usuarios");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            CreatedAt = new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2763),
                            IsDeleted = false,
                            Password = "$2a$12$ATuj2Hpw./1z0jTlWJhnRO2BtV50WycRxH8WdsN3VnCw.5t4Phph6",
                            Role = 1,
                            UpdatedAt = new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2763),
                            Username = "pedrito"
                        },
                        new
                        {
                            Id = 2L,
                            CreatedAt = new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2781),
                            IsDeleted = false,
                            Password = "$2a$12$Q6XFDZIrRI5O.kxOoCFmIebwZRSmSlRg81el0Sa4WYm5wmhwCgSyq",
                            Role = 0,
                            UpdatedAt = new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2782),
                            Username = "anita"
                        });
                });

            modelBuilder.Entity("examn.Database.Entities.FunkoEntity", b =>
                {
                    b.HasOne("examn.Database.Entities.CategoriaEntity", "Categoria")
                        .WithMany()
                        .HasForeignKey("CategoriaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Categoria");
                });
#pragma warning restore 612, 618
        }
    }
}
