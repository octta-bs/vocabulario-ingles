using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabularioIngles.Data.Migraciones
{
    /// <inheritdoc />
    public partial class InicioBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Palabras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ingles = table.Column<string>(type: "TEXT", nullable: false),
                    Espanol = table.Column<string>(type: "TEXT", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Pronunciacion = table.Column<string>(type: "TEXT", nullable: false),
                    ComoSeLee = table.Column<string>(type: "TEXT", nullable: false),
                    Pasado = table.Column<string>(type: "TEXT", nullable: true),
                    Participio = table.Column<string>(type: "TEXT", nullable: true),
                    Ejemplo = table.Column<string>(type: "TEXT", nullable: false),
                    EjemploEs = table.Column<string>(type: "TEXT", nullable: false),
                    Nota = table.Column<string>(type: "TEXT", nullable: true),
                    CategoriaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Palabras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Palabras_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Progresos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PalabraId = table.Column<int>(type: "INTEGER", nullable: false),
                    Etapa = table.Column<int>(type: "INTEGER", nullable: false),
                    ProximoRepaso = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UltimoRepaso = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Aciertos = table.Column<int>(type: "INTEGER", nullable: false),
                    Fallos = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Progresos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Progresos_Palabras_PalabraId",
                        column: x => x.PalabraId,
                        principalTable: "Palabras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Palabras_CategoriaId",
                table: "Palabras",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Progresos_PalabraId",
                table: "Progresos",
                column: "PalabraId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Progresos");

            migrationBuilder.DropTable(
                name: "Palabras");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
