using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingDb.Migrations
{
    public partial class CaptchaUpdate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelId",
                table: "Guilds_Captcha");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelId",
                table: "Guilds_Captcha",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelId",
                table: "Guilds_Captcha");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelId",
                table: "Guilds_Captcha",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
