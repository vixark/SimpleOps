using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleOps.Migrations
{
    public partial class dos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cude",
                table: "Ventas",
                maxLength: 96,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cude",
                table: "NotasDébitoVenta",
                maxLength: 96,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cude",
                table: "NotasDébitoCompra",
                maxLength: 96,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cude",
                table: "NotasCréditoVenta",
                maxLength: 96,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cude",
                table: "NotasCréditoCompra",
                maxLength: 96,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cude",
                table: "Compras",
                maxLength: 96,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cude",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Cude",
                table: "NotasDébitoVenta");

            migrationBuilder.DropColumn(
                name: "Cude",
                table: "NotasDébitoCompra");

            migrationBuilder.DropColumn(
                name: "Cude",
                table: "NotasCréditoVenta");

            migrationBuilder.DropColumn(
                name: "Cude",
                table: "NotasCréditoCompra");

            migrationBuilder.DropColumn(
                name: "Cude",
                table: "Compras");
        }
    }
}
