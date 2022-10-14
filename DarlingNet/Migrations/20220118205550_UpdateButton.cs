using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarlingNet.Migrations
{
    public partial class UpdateButton : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ButtonClick_Channels_ChannelId",
                table: "ButtonClick");

            migrationBuilder.AlterColumn<ulong>(
                name: "ChannelId",
                table: "ButtonClick",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ButtonClick_Channels_ChannelId",
                table: "ButtonClick",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ButtonClick_Channels_ChannelId",
                table: "ButtonClick");

            migrationBuilder.AlterColumn<ulong>(
                name: "ChannelId",
                table: "ButtonClick",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_ButtonClick_Channels_ChannelId",
                table: "ButtonClick",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id");
        }
    }
}
