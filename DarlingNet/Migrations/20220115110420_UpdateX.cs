using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarlingNet.Migrations
{
    public partial class UpdateX : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Guilds_GuildsId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Timer_Guilds_GuildsId",
                table: "Roles_Timer");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Timer_GuildsId",
                table: "Roles_Timer");

            migrationBuilder.DropIndex(
                name: "IX_Roles_GuildsId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "GuildsId",
                table: "Roles_Timer");

            migrationBuilder.DropColumn(
                name: "GuildsId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "WelcomeRole",
                table: "Guilds_Meeting_Welcome");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ButtonClick",
                newName: "RoleId");

            migrationBuilder.AddColumn<ulong>(
                name: "WelcomeRoleId",
                table: "Guilds_Meeting_Welcome",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RoleDelOrGet",
                table: "ButtonClick",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Timer_RoleId",
                table: "Roles_Timer",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleId",
                table: "Roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_Meeting_Welcome_WelcomeRoleId",
                table: "Guilds_Meeting_Welcome",
                column: "WelcomeRoleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmoteClick_RoleId",
                table: "EmoteClick",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ButtonClick_RoleId",
                table: "ButtonClick",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_GuildId",
                table: "Role",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_ButtonClick_Role_RoleId",
                table: "ButtonClick",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmoteClick_Role_RoleId",
                table: "EmoteClick",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Meeting_Welcome_Role_WelcomeRoleId",
                table: "Guilds_Meeting_Welcome",
                column: "WelcomeRoleId",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Role_RoleId",
                table: "Roles",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Timer_Role_RoleId",
                table: "Roles_Timer",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ButtonClick_Role_RoleId",
                table: "ButtonClick");

            migrationBuilder.DropForeignKey(
                name: "FK_EmoteClick_Role_RoleId",
                table: "EmoteClick");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Meeting_Welcome_Role_WelcomeRoleId",
                table: "Guilds_Meeting_Welcome");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Role_RoleId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Timer_Role_RoleId",
                table: "Roles_Timer");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Timer_RoleId",
                table: "Roles_Timer");

            migrationBuilder.DropIndex(
                name: "IX_Roles_RoleId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Guilds_Meeting_Welcome_WelcomeRoleId",
                table: "Guilds_Meeting_Welcome");

            migrationBuilder.DropIndex(
                name: "IX_EmoteClick_RoleId",
                table: "EmoteClick");

            migrationBuilder.DropIndex(
                name: "IX_ButtonClick_RoleId",
                table: "ButtonClick");

            migrationBuilder.DropColumn(
                name: "WelcomeRoleId",
                table: "Guilds_Meeting_Welcome");

            migrationBuilder.DropColumn(
                name: "RoleDelOrGet",
                table: "ButtonClick");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "ButtonClick",
                newName: "Type");

            migrationBuilder.AddColumn<ulong>(
                name: "GuildsId",
                table: "Roles_Timer",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "GuildsId",
                table: "Roles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "WelcomeRole",
                table: "Guilds_Meeting_Welcome",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Timer_GuildsId",
                table: "Roles_Timer",
                column: "GuildsId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_GuildsId",
                table: "Roles",
                column: "GuildsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Guilds_GuildsId",
                table: "Roles",
                column: "GuildsId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Timer_Guilds_GuildsId",
                table: "Roles_Timer",
                column: "GuildsId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
