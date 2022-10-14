using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class GiveAwaysUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiveAways_Guilds_GuildId",
                table: "GiveAways");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "GiveAways",
                newName: "ChannelsId");

            migrationBuilder.RenameIndex(
                name: "IX_GiveAways_GuildId",
                table: "GiveAways",
                newName: "IX_GiveAways_ChannelsId");

            migrationBuilder.AddColumn<string>(
                name: "Surpice",
                table: "GiveAways",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GiveAways_Channels_ChannelsId",
                table: "GiveAways",
                column: "ChannelsId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiveAways_Channels_ChannelsId",
                table: "GiveAways");

            migrationBuilder.DropColumn(
                name: "Surpice",
                table: "GiveAways");

            migrationBuilder.RenameColumn(
                name: "ChannelsId",
                table: "GiveAways",
                newName: "GuildId");

            migrationBuilder.RenameIndex(
                name: "IX_GiveAways_ChannelsId",
                table: "GiveAways",
                newName: "IX_GiveAways_GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_GiveAways_Guilds_GuildId",
                table: "GiveAways",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
