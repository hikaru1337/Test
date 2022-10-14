using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class UpdateDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Invise",
                table: "TempUser");

            migrationBuilder.DropColumn(
                name: "VoiceChannelId",
                table: "PrivateChannels");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Invise",
                table: "TempUser",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<ulong>(
                name: "VoiceChannelId",
                table: "PrivateChannels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }
    }
}
