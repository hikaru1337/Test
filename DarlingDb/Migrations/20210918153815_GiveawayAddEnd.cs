using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingDb.Migrations
{
    public partial class GiveawayAddEnd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdminEnd",
                table: "GiveAways",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminEnd",
                table: "GiveAways");
        }
    }
}
