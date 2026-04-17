using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlataformaReservas.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AddImagenUrlToPropiedad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Propiedades",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Propiedades");
        }
    }
}
