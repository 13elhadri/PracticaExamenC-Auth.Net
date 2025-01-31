using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace examn.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Funkos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    categoria_id = table.Column<long>(type: "bigint", nullable: false),
                    Foto = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funkos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Funkos_Categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "Nombre", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2914), false, "Categoría 1", new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2914) },
                    { 2L, new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2916), false, "Categoría 2", new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2916) }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "Password", "Role", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2763), false, "$2a$12$ATuj2Hpw./1z0jTlWJhnRO2BtV50WycRxH8WdsN3VnCw.5t4Phph6", 1, new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2763), "pedrito" },
                    { 2L, new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2781), false, "$2a$12$Q6XFDZIrRI5O.kxOoCFmIebwZRSmSlRg81el0Sa4WYm5wmhwCgSyq", 0, new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(2782), "anita" }
                });

            migrationBuilder.InsertData(
                table: "Funkos",
                columns: new[] { "Id", "categoria_id", "CreatedAt", "Foto", "IsDeleted", "Nombre", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, 1L, new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(3881), "spiderman_foto_url", false, "Funko Pop - Spider-Man", new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(3882) },
                    { 2L, 2L, new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(3883), "batman_foto_url", false, "Funko Pop - Batman", new DateTime(2025, 1, 31, 11, 7, 20, 762, DateTimeKind.Utc).AddTicks(3884) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Funkos_categoria_id",
                table: "Funkos",
                column: "categoria_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Funkos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
