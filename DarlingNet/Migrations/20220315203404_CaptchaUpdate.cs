using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class CaptchaUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelId",
                table: "Guilds_Captcha");

            migrationBuilder.DropIndex(
                name: "IX_Guilds_Captcha_RoleId",
                table: "Guilds_Captcha");

            migrationBuilder.RenameColumn(
                name: "Get",
                table: "Guilds_Captcha",
                newName: "GuildId");

            migrationBuilder.AlterColumn<ulong>(
                name: "Id",
                table: "Role",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<ulong>(
                name: "ChannelId",
                table: "Guilds_Captcha",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<ulong>(
                name: "Id",
                table: "Channels",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_Captcha_GuildId",
                table: "Guilds_Captcha",
                column: "GuildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_Captcha_RoleId",
                table: "Guilds_Captcha",
                column: "RoleId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelId",
                table: "Guilds_Captcha",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Captcha_Guilds_GuildId",
                table: "Guilds_Captcha",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelId",
                table: "Guilds_Captcha");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Captcha_Guilds_GuildId",
                table: "Guilds_Captcha");

            migrationBuilder.DropIndex(
                name: "IX_Guilds_Captcha_GuildId",
                table: "Guilds_Captcha");

            migrationBuilder.DropIndex(
                name: "IX_Guilds_Captcha_RoleId",
                table: "Guilds_Captcha");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "Guilds_Captcha",
                newName: "Get");

            migrationBuilder.AlterColumn<ulong>(
                name: "Id",
                table: "Role",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<ulong>(
                name: "ChannelId",
                table: "Guilds_Captcha",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "Id",
                table: "Channels",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_Captcha_RoleId",
                table: "Guilds_Captcha",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelId",
                table: "Guilds_Captcha",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
