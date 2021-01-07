using Microsoft.EntityFrameworkCore.Migrations;

namespace Coza.DataAccess.Migrations
{
    public partial class UpdateOrderHeaderDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "OrderHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "OrderHeader",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FlwRef",
                table: "OrderHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TxRef",
                table: "OrderHeader",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "FlwRef",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "TxRef",
                table: "OrderHeader");
        }
    }
}
