using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class CaptchaUpdate4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Captcha_Role_RoleId",
                table: "Guilds_Captcha");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Captcha_Role_RoleId",
                table: "Guilds_Captcha",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Captcha_Role_RoleId",
                table: "Guilds_Captcha");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Captcha_Role_RoleId",
                table: "Guilds_Captcha",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
