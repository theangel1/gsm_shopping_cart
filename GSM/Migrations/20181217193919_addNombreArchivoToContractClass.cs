using Microsoft.EntityFrameworkCore.Migrations;

namespace GSM.Migrations
{
    public partial class addNombreArchivoToContractClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetodoPago",
                table: "Contract");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Contract",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Rut",
                table: "Contract",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "RazonSocial",
                table: "Contract",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Contract",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "NombreArchivo",
                table: "Contract",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreArchivo",
                table: "Contract");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Contract",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Rut",
                table: "Contract",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RazonSocial",
                table: "Contract",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Contract",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetodoPago",
                table: "Contract",
                nullable: false,
                defaultValue: "");
        }
    }
}
