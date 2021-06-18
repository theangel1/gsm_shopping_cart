using Microsoft.EntityFrameworkCore.Migrations;

namespace GSM.Migrations
{
    public partial class addFieldImagenPlanoToTableBuilding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenPlano",
                table: "Building",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenPlano",
                table: "Building");
        }
    }
}
