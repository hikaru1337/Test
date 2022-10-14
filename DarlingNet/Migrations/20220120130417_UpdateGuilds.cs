using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarlingNet.Migrations
{
    public partial class UpdateGuilds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldMenu",
                table: "Guilds");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OldMenu",
                table: "Guilds",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
