using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabularioIngles.Data.Migraciones
{
    /// <inheritdoc />
    public partial class ModoInfantil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerfilesInfantiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilesInfantiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgresosInfantiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PalabraId = table.Column<int>(type: "INTEGER", nullable: false),
                    Aciertos = table.Column<int>(type: "INTEGER", nullable: false),
                    Fallos = table.Column<int>(type: "INTEGER", nullable: false),
                    AciertosSeguidos = table.Column<int>(type: "INTEGER", nullable: false),
                    UltimaVez = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgresosInfantiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgresosInfantiles_Palabras_PalabraId",
                        column: x => x.PalabraId,
                        principalTable: "Palabras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgresosInfantiles_PalabraId",
                table: "ProgresosInfantiles",
                column: "PalabraId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerfilesInfantiles");

            migrationBuilder.DropTable(
                name: "ProgresosInfantiles");
        }
    }
}
