using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class ReportUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports_PunishesUsers_Guild",
                columns: table => new
                {
                    Reports_PunishesId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Users_GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports_PunishesUsers_Guild", x => new { x.Reports_PunishesId, x.Users_GuildId });
                    table.ForeignKey(
                        name: "FK_Reports_PunishesUsers_Guild_Reports_Punishes_Reports_PunishesId",
                        column: x => x.Reports_PunishesId,
                        principalTable: "Reports_Punishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_PunishesUsers_Guild_Users_Guild_Users_GuildId",
                        column: x => x.Users_GuildId,
                        principalTable: "Users_Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_PunishesUsers_Guild_Users_GuildId",
                table: "Reports_PunishesUsers_Guild",
                column: "Users_GuildId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reports_PunishesUsers_Guild");
        }
    }
}
