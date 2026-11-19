using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabularioIngles.Data.Migraciones
{
    /// <inheritdoc />
    public partial class Gramatica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FraseIncorrecta",
                table: "Palabras",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FraseIncorrecta",
                table: "Palabras");
        }
    }
}
