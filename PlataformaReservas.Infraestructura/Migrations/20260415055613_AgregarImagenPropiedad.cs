using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlataformaReservas.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AgregarImagenPropiedad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenPrincipalUrl",
                table: "Propiedades",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenPrincipalUrl",
                table: "Propiedades");
        }
    }
}
