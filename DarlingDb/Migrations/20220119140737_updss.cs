using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarlingDb.Migrations
{
    public partial class updss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ButtonText",
                table: "ButtonClick");

            migrationBuilder.RenameColumn(
                name: "RaidStop",
                table: "Guilds_Raid",
                newName: "RaidRunning");

            migrationBuilder.AddColumn<string>(
                name: "AdminMessage",
                table: "Feedback",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminMessage",
                table: "Feedback");

            migrationBuilder.RenameColumn(
                name: "RaidRunning",
                table: "Guilds_Raid",
                newName: "RaidStop");

            migrationBuilder.AddColumn<string>(
                name: "ButtonText",
                table: "ButtonClick",
                type: "TEXT",
                nullable: true);
        }
    }
}
