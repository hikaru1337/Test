using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingDb.Migrations
{
    public partial class RenameColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Guilds_GuildId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_Guilds_GuildId",
                table: "Role");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "Role",
                newName: "GuildsId");

            migrationBuilder.RenameIndex(
                name: "IX_Role_GuildId",
                table: "Role",
                newName: "IX_Role_GuildsId");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "Reports",
                newName: "GuildsId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_GuildId",
                table: "Reports",
                newName: "IX_Reports_GuildsId");

            migrationBuilder.CreateTable(
                name: "Guilds_Captcha",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Get = table.Column<bool>(type: "INTEGER", nullable: false),
                    Run = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds_Captcha", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guilds_Captcha_Channels_ChannelsId",
                        column: x => x.ChannelsId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Guilds_Captcha_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_Captcha_ChannelsId",
                table: "Guilds_Captcha",
                column: "ChannelsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_Captcha_RoleId",
                table: "Guilds_Captcha",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Guilds_GuildsId",
                table: "Reports",
                column: "GuildsId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Guilds_GuildsId",
                table: "Role",
                column: "GuildsId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Guilds_GuildsId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_Guilds_GuildsId",
                table: "Role");

            migrationBuilder.DropTable(
                name: "Guilds_Captcha");

            migrationBuilder.RenameColumn(
                name: "GuildsId",
                table: "Role",
                newName: "GuildId");

            migrationBuilder.RenameIndex(
                name: "IX_Role_GuildsId",
                table: "Role",
                newName: "IX_Role_GuildId");

            migrationBuilder.RenameColumn(
                name: "GuildsId",
                table: "Reports",
                newName: "GuildId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_GuildsId",
                table: "Reports",
                newName: "IX_Reports_GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Guilds_GuildId",
                table: "Reports",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Guilds_GuildId",
                table: "Role",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
