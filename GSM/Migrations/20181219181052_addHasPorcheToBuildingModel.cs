using Microsoft.EntityFrameworkCore.Migrations;

namespace GSM.Migrations
{
    public partial class addHasPorcheToBuildingModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasPorche",
                table: "Building",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPorche",
                table: "Building");
        }
    }
}
