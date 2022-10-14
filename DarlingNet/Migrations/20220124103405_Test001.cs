using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class Test001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DarlingBoost_Users_UserId",
                table: "DarlingBoost");

            migrationBuilder.DropIndex(
                name: "IX_DarlingBoost_UserId",
                table: "DarlingBoost");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DarlingBoost");

            migrationBuilder.AddColumn<ulong>(
                name: "BoostId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "Coins",
                table: "Users",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BoostId",
                table: "Users",
                column: "BoostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_DarlingBoost_BoostId",
                table: "Users",
                column: "BoostId",
                principalTable: "DarlingBoost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_DarlingBoost_BoostId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BoostId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BoostId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Coins",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Users");

            migrationBuilder.AddColumn<ulong>(
                name: "UserId",
                table: "DarlingBoost",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateIndex(
                name: "IX_DarlingBoost_UserId",
                table: "DarlingBoost",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DarlingBoost_Users_UserId",
                table: "DarlingBoost",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
