using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabularioIngles.Data.Migraciones
{
    /// <inheritdoc />
    public partial class NivelesMcer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Nivel",
                table: "Categorias",
                type: "INTEGER",
                nullable: false,
                defaultValue: 2); // A2: el nivel de las palabras de tecnología y trabajo que ya existían

            // Las frases de trabajo ya existentes son de nivel intermedio (B1); la entrevista técnica, B2.
            migrationBuilder.Sql(
                "UPDATE Categorias SET Nivel = 3 WHERE Nombre IN " +
                "('Stand-up', 'Code review', 'Entrevista', 'Git y GitHub', 'Agile y Scrum', 'Reuniones y correo');");
            migrationBuilder.Sql("UPDATE Categorias SET Nivel = 4 WHERE Nombre = 'Entrevista técnica';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nivel",
                table: "Categorias");
        }
    }
}
