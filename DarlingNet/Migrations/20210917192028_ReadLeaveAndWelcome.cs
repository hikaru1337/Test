using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class ReadLeaveAndWelcome : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Meeting_Leave_Channels_LeaveChannelsId",
                table: "Guilds_Meeting_Leave");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Meeting_Welcome_Channels_WelcomeChannelsId",
                table: "Guilds_Meeting_Welcome");

            migrationBuilder.AlterColumn<ulong>(
                name: "WelcomeChannelsId",
                table: "Guilds_Meeting_Welcome",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "LeaveChannelsId",
                table: "Guilds_Meeting_Leave",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Meeting_Leave_Channels_LeaveChannelsId",
                table: "Guilds_Meeting_Leave");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Meeting_Welcome_Channels_WelcomeChannelsId",
                table: "Guilds_Meeting_Welcome");

            migrationBuilder.AlterColumn<ulong>(
                name: "WelcomeChannelsId",
                table: "Guilds_Meeting_Welcome",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<ulong>(
                name: "LeaveChannelsId",
                table: "Guilds_Meeting_Leave",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Meeting_Leave_Channels_LeaveChannelsId",
                table: "Guilds_Meeting_Leave",
                column: "LeaveChannelsId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Meeting_Welcome_Channels_WelcomeChannelsId",
                table: "Guilds_Meeting_Welcome",
                column: "WelcomeChannelsId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
