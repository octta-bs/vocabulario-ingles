using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabularioIngles.Data.Migraciones
{
    /// <inheritdoc />
    public partial class RespuestasPropias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RespuestasPropias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClavePregunta = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Texto = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Actualizada = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespuestasPropias", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasPropias_ClavePregunta",
                table: "RespuestasPropias",
                column: "ClavePregunta",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RespuestasPropias");
        }
    }
}
