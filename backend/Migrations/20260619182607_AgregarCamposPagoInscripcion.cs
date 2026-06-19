using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventosAPI.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposPagoInscripcion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PagoConfirmado",
                table: "Inscripciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReferenciaPago",
                table: "Inscripciones",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PagoConfirmado",
                table: "Inscripciones");

            migrationBuilder.DropColumn(
                name: "ReferenciaPago",
                table: "Inscripciones");
        }
    }
}
