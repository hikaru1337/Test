using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarlingNet.Migrations
{
    public partial class updateGuild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TempUser_Channels_ChannelsId",
                table: "TempUser");

            migrationBuilder.DropColumn(
                name: "ChatMuteRole",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "VoiceMuteRole",
                table: "Guilds");

            migrationBuilder.AlterColumn<int>(
                name: "BirthDateComplete",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "ChannelsId",
                table: "TempUser",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<ulong>(
                name: "ChannelId",
                table: "Invites",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invites_ChannelId",
                table: "Invites",
                column: "ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_Channels_ChannelId",
                table: "Invites",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TempUser_Channels_ChannelsId",
                table: "TempUser",
                column: "ChannelsId",
                principalTable: "Channels",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_Channels_ChannelId",
                table: "Invites");

            migrationBuilder.DropForeignKey(
                name: "FK_TempUser_Channels_ChannelsId",
                table: "TempUser");

            migrationBuilder.DropIndex(
                name: "IX_Invites_ChannelId",
                table: "Invites");

            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "Invites");

            migrationBuilder.AlterColumn<string>(
                name: "BirthDateComplete",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<ulong>(
                name: "ChannelsId",
                table: "TempUser",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "ChatMuteRole",
                table: "Guilds",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "VoiceMuteRole",
                table: "Guilds",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddForeignKey(
                name: "FK_TempUser_Channels_ChannelsId",
                table: "TempUser",
                column: "ChannelsId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
