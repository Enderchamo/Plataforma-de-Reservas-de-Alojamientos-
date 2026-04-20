using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlataformaReservas.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRelacionHostPropiedadCorregida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Propiedades_Usuarios_UsuarioId",
                table: "Propiedades");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Propiedades_PropiedadId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Usuarios_UsuarioInvitadoId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Propiedades_UsuarioId",
                table: "Propiedades");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Propiedades");

            migrationBuilder.CreateIndex(
                name: "IX_Propiedades_HostId",
                table: "Propiedades",
                column: "HostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Propiedades_Usuarios_HostId",
                table: "Propiedades",
                column: "HostId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Propiedades_PropiedadId",
                table: "Reservas",
                column: "PropiedadId",
                principalTable: "Propiedades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Usuarios_UsuarioInvitadoId",
                table: "Reservas",
                column: "UsuarioInvitadoId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Propiedades_Usuarios_HostId",
                table: "Propiedades");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Propiedades_PropiedadId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Usuarios_UsuarioInvitadoId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Propiedades_HostId",
                table: "Propiedades");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Propiedades",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Propiedades_UsuarioId",
                table: "Propiedades",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Propiedades_Usuarios_UsuarioId",
                table: "Propiedades",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Propiedades_PropiedadId",
                table: "Reservas",
                column: "PropiedadId",
                principalTable: "Propiedades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Usuarios_UsuarioInvitadoId",
                table: "Reservas",
                column: "UsuarioInvitadoId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
