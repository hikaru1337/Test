using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingDb.Migrations
{
    public partial class MoreRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmoteClick_Channels_ChannelsId",
                table: "EmoteClick");

            migrationBuilder.DropForeignKey(
                name: "FK_GiveAways_Channels_ChannelsId",
                table: "GiveAways");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelsId",
                table: "Guilds_Captcha");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Logs_Channels_ChannelsId",
                table: "Guilds_Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Meeting_Leave_Channels_LeaveChannelsId",
                table: "Guilds_Meeting_Leave");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Meeting_Welcome_Channels_WelcomeChannelsId",
                table: "Guilds_Meeting_Welcome");

            migrationBuilder.DropForeignKey(
                name: "FK_Invites_Channels_ChannelId",
                table: "Invites");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Channels_ChannelsId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TempUser_Channels_ChannelsId",
                table: "TempUser");

            migrationBuilder.DropIndex(
                name: "IX_TempUser_ChannelsId",
                table: "TempUser");

            migrationBuilder.DropColumn(
                name: "ChannelsId",
                table: "TempUser");

            migrationBuilder.RenameColumn(
                name: "ChannelsId",
                table: "Tasks",
                newName: "ChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_ChannelsId",
                table: "Tasks",
                newName: "IX_Tasks_ChannelId");

            migrationBuilder.RenameColumn(
                name: "WelcomeChannelsId",
                table: "Guilds_Meeting_Welcome",
                newName: "WelcomeChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_Guilds_Meeting_Welcome_WelcomeChannelsId",
                table: "Guilds_Meeting_Welcome",
                newName: "IX_Guilds_Meeting_Welcome_WelcomeChannelId");

            migrationBuilder.RenameColumn(
                name: "LeaveChannelsId",
                table: "Guilds_Meeting_Leave",
                newName: "LeaveChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_Guilds_Meeting_Leave_LeaveChannelsId",
                table: "Guilds_Meeting_Leave",
                newName: "IX_Guilds_Meeting_Leave_LeaveChannelId");

            migrationBuilder.RenameColumn(
                name: "ChannelsId",
                table: "Guilds_Logs",
                newName: "ChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_Guilds_Logs_ChannelsId",
                table: "Guilds_Logs",
                newName: "IX_Guilds_Logs_ChannelId");

            migrationBuilder.RenameColumn(
                name: "ChannelsId",
                table: "Guilds_Captcha",
                newName: "ChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_Guilds_Captcha_ChannelsId",
                table: "Guilds_Captcha",
                newName: "IX_Guilds_Captcha_ChannelId");

            migrationBuilder.RenameColumn(
                name: "ChannelsId",
                table: "GiveAways",
                newName: "ChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_GiveAways_ChannelsId",
                table: "GiveAways",
                newName: "IX_GiveAways_ChannelId");

            migrationBuilder.RenameColumn(
                name: "ChannelsId",
                table: "EmoteClick",
                newName: "ChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_EmoteClick_ChannelsId",
                table: "EmoteClick",
                newName: "IX_EmoteClick_ChannelId");

            migrationBuilder.AlterColumn<ulong>(
                name: "ChannelId",
                table: "Invites",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmoteClick_Channels_ChannelId",
                table: "EmoteClick",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GiveAways_Channels_ChannelId",
                table: "GiveAways",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelId",
                table: "Guilds_Captcha",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Logs_Channels_ChannelId",
                table: "Guilds_Logs",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Meeting_Leave_Channels_LeaveChannelId",
                table: "Guilds_Meeting_Leave",
                column: "LeaveChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Meeting_Welcome_Channels_WelcomeChannelId",
                table: "Guilds_Meeting_Welcome",
                column: "WelcomeChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_Channels_ChannelId",
                table: "Invites",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Channels_ChannelId",
                table: "Tasks",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmoteClick_Channels_ChannelId",
                table: "EmoteClick");

            migrationBuilder.DropForeignKey(
                name: "FK_GiveAways_Channels_ChannelId",
                table: "GiveAways");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelId",
                table: "Guilds_Captcha");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Logs_Channels_ChannelId",
                table: "Guilds_Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Meeting_Leave_Channels_LeaveChannelId",
                table: "Guilds_Meeting_Leave");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Meeting_Welcome_Channels_WelcomeChannelId",
                table: "Guilds_Meeting_Welcome");

            migrationBuilder.DropForeignKey(
                name: "FK_Invites_Channels_ChannelId",
                table: "Invites");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Channels_ChannelId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "ChannelId",
                table: "Tasks",
                newName: "ChannelsId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_ChannelId",
                table: "Tasks",
                newName: "IX_Tasks_ChannelsId");

            migrationBuilder.RenameColumn(
                name: "WelcomeChannelId",
                table: "Guilds_Meeting_Welcome",
                newName: "WelcomeChannelsId");

            migrationBuilder.RenameIndex(
                name: "IX_Guilds_Meeting_Welcome_WelcomeChannelId",
                table: "Guilds_Meeting_Welcome",
                newName: "IX_Guilds_Meeting_Welcome_WelcomeChannelsId");

            migrationBuilder.RenameColumn(
                name: "LeaveChannelId",
                table: "Guilds_Meeting_Leave",
                newName: "LeaveChannelsId");

            migrationBuilder.RenameIndex(
                name: "IX_Guilds_Meeting_Leave_LeaveChannelId",
                table: "Guilds_Meeting_Leave",
                newName: "IX_Guilds_Meeting_Leave_LeaveChannelsId");

            migrationBuilder.RenameColumn(
                name: "ChannelId",
                table: "Guilds_Logs",
                newName: "ChannelsId");

            migrationBuilder.RenameIndex(
                name: "IX_Guilds_Logs_ChannelId",
                table: "Guilds_Logs",
                newName: "IX_Guilds_Logs_ChannelsId");

            migrationBuilder.RenameColumn(
                name: "ChannelId",
                table: "Guilds_Captcha",
                newName: "ChannelsId");

            migrationBuilder.RenameIndex(
                name: "IX_Guilds_Captcha_ChannelId",
                table: "Guilds_Captcha",
                newName: "IX_Guilds_Captcha_ChannelsId");

            migrationBuilder.RenameColumn(
                name: "ChannelId",
                table: "GiveAways",
                newName: "ChannelsId");

            migrationBuilder.RenameIndex(
                name: "IX_GiveAways_ChannelId",
                table: "GiveAways",
                newName: "IX_GiveAways_ChannelsId");

            migrationBuilder.RenameColumn(
                name: "ChannelId",
                table: "EmoteClick",
                newName: "ChannelsId");

            migrationBuilder.RenameIndex(
                name: "IX_EmoteClick_ChannelId",
                table: "EmoteClick",
                newName: "IX_EmoteClick_ChannelsId");

            migrationBuilder.AddColumn<ulong>(
                name: "ChannelsId",
                table: "TempUser",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "ChannelId",
                table: "Invites",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_TempUser_ChannelsId",
                table: "TempUser",
                column: "ChannelsId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmoteClick_Channels_ChannelsId",
                table: "EmoteClick",
                column: "ChannelsId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GiveAways_Channels_ChannelsId",
                table: "GiveAways",
                column: "ChannelsId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelsId",
                table: "Guilds_Captcha",
                column: "ChannelsId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Logs_Channels_ChannelsId",
                table: "Guilds_Logs",
                column: "ChannelsId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Meeting_Leave_Channels_LeaveChannelsId",
                table: "Guilds_Meeting_Leave",
                column: "LeaveChannelsId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Meeting_Welcome_Channels_WelcomeChannelsId",
                table: "Guilds_Meeting_Welcome",
                column: "WelcomeChannelsId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_Channels_ChannelId",
                table: "Invites",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Channels_ChannelsId",
                table: "Tasks",
                column: "ChannelsId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TempUser_Channels_ChannelsId",
                table: "TempUser",
                column: "ChannelsId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
