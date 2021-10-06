using Microsoft.EntityFrameworkCore.Migrations;

namespace PrivTours.Migrations
{
    public partial class JQaddnewfieldsfixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "tipoContrato",
                table: "Empleados",
                newName: "TipoContrato");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TipoContrato",
                table: "Empleados",
                newName: "tipoContrato");
        }
    }
}
