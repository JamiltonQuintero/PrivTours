using Microsoft.EntityFrameworkCore.Migrations;

namespace PrivTours.Migrations
{
    public partial class JQaddnewfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Apellido",
                table: "Empleados",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Celular",
                table: "Empleados",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tipoContrato",
                table: "Empleados",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Apellido",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "Celular",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "tipoContrato",
                table: "Empleados");
        }
    }
}
