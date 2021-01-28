using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabasFinal.Migrations
{
    public partial class V2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndoorTemp",
                table: "Tempratures");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Tempratures",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Temperature",
                table: "Tempratures",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Tempratures");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "Tempratures");

            migrationBuilder.AddColumn<double>(
                name: "IndoorTemp",
                table: "Tempratures",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
