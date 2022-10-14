using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarlingNet.Migrations
{
    public partial class upds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ButtonClick_Channels_ChannelsId",
                table: "ButtonClick");

            migrationBuilder.DropForeignKey(
                name: "FK_ButtonClick_Role_RoleId",
                table: "ButtonClick");

            migrationBuilder.DropIndex(
                name: "IX_ButtonClick_ChannelsId",
                table: "ButtonClick");

            migrationBuilder.DropColumn(
                name: "ChannelsId",
                table: "ButtonClick");

            migrationBuilder.RenameColumn(
                name: "Style",
                table: "ButtonClick",
                newName: "MessageId");

            migrationBuilder.AlterColumn<ulong>(
                name: "RoleId",
                table: "ButtonClick",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<ulong>(
                name: "ChannelId",
                table: "ButtonClick",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ButtonClick_ChannelId",
                table: "ButtonClick",
                column: "ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ButtonClick_Channels_ChannelId",
                table: "ButtonClick",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ButtonClick_Role_RoleId",
                table: "ButtonClick",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ButtonClick_Channels_ChannelId",
                table: "ButtonClick");

            migrationBuilder.DropForeignKey(
                name: "FK_ButtonClick_Role_RoleId",
                table: "ButtonClick");

            migrationBuilder.DropIndex(
                name: "IX_ButtonClick_ChannelId",
                table: "ButtonClick");

            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "ButtonClick");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "ButtonClick",
                newName: "Style");

            migrationBuilder.AlterColumn<ulong>(
                name: "RoleId",
                table: "ButtonClick",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "ChannelsId",
                table: "ButtonClick",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateIndex(
                name: "IX_ButtonClick_ChannelsId",
                table: "ButtonClick",
                column: "ChannelsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ButtonClick_Channels_ChannelsId",
                table: "ButtonClick",
                column: "ChannelsId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ButtonClick_Role_RoleId",
                table: "ButtonClick",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
