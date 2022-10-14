using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingDb.Migrations
{
    public partial class DeleteUserAndInviteConnect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_Users_UsersId",
                table: "Invites");

            migrationBuilder.DropIndex(
                name: "IX_Invites_UsersId",
                table: "Invites");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Invites_UsersId",
                table: "Invites",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_Users_UsersId",
                table: "Invites",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
