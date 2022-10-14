using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingDb.Migrations
{
    public partial class Test_UserUpdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_Users_UserId",
                table: "Invites");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateChannels_Users_UsersId",
                table: "PrivateChannels");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Timer_Users_UsersId",
                table: "Roles_Timer");

            migrationBuilder.DropForeignKey(
                name: "FK_TempUser_Users_UsersId",
                table: "TempUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_DarlingBoost_BoostId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Guilds_GuildsId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_UsersMId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BoostId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_GuildsId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UsersMId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BirthDateInvise",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BoostId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Coins",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CountWarns",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Daily",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GuildsId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Leaved",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReportedRules",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Streak",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UsersMId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VoiceActive",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "XP",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ZeroCoin",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "TempUser",
                newName: "Users_GuildId");

            migrationBuilder.RenameIndex(
                name: "IX_TempUser_UsersId",
                table: "TempUser",
                newName: "IX_TempUser_Users_GuildId");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "Roles_Timer",
                newName: "Users_GuildId");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_Timer_UsersId",
                table: "Roles_Timer",
                newName: "IX_Roles_Timer_Users_GuildId");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "PrivateChannels",
                newName: "Users_GuildId");

            migrationBuilder.RenameIndex(
                name: "IX_PrivateChannels_UsersId",
                table: "PrivateChannels",
                newName: "IX_PrivateChannels_Users_GuildId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Invites",
                newName: "Users_GuildId");

            migrationBuilder.RenameIndex(
                name: "IX_Invites_UserId",
                table: "Invites",
                newName: "IX_Invites_Users_GuildId");

            migrationBuilder.AddColumn<ulong>(
                name: "UsersId",
                table: "DarlingBoost",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateTable(
                name: "Users_Guild",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsersId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    GuildsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    VoiceActive = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    UsersMId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Leaved = table.Column<bool>(type: "INTEGER", nullable: false),
                    XP = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ZeroCoin = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Daily = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Streak = table.Column<ulong>(type: "INTEGER", nullable: false),
                    CountWarns = table.Column<uint>(type: "INTEGER", nullable: false),
                    ReportedRules = table.Column<string>(type: "TEXT", nullable: true),
                    BirthDateInvise = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_Guild", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Guild_Guilds_GuildsId",
                        column: x => x.GuildsId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Guild_Users_Guild_UsersMId",
                        column: x => x.UsersMId,
                        principalTable: "Users_Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Guild_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DarlingBoost_UsersId",
                table: "DarlingBoost",
                column: "UsersId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Guild_GuildsId",
                table: "Users_Guild",
                column: "GuildsId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Guild_UsersId",
                table: "Users_Guild",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Guild_UsersMId",
                table: "Users_Guild",
                column: "UsersMId");

            migrationBuilder.AddForeignKey(
                name: "FK_DarlingBoost_Users_UsersId",
                table: "DarlingBoost",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_Users_Guild_Users_GuildId",
                table: "Invites",
                column: "Users_GuildId",
                principalTable: "Users_Guild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateChannels_Users_Guild_Users_GuildId",
                table: "PrivateChannels",
                column: "Users_GuildId",
                principalTable: "Users_Guild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Timer_Users_Guild_Users_GuildId",
                table: "Roles_Timer",
                column: "Users_GuildId",
                principalTable: "Users_Guild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TempUser_Users_Guild_Users_GuildId",
                table: "TempUser",
                column: "Users_GuildId",
                principalTable: "Users_Guild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DarlingBoost_Users_UsersId",
                table: "DarlingBoost");

            migrationBuilder.DropForeignKey(
                name: "FK_Invites_Users_Guild_Users_GuildId",
                table: "Invites");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateChannels_Users_Guild_Users_GuildId",
                table: "PrivateChannels");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Timer_Users_Guild_Users_GuildId",
                table: "Roles_Timer");

            migrationBuilder.DropForeignKey(
                name: "FK_TempUser_Users_Guild_Users_GuildId",
                table: "TempUser");

            migrationBuilder.DropTable(
                name: "Users_Guild");

            migrationBuilder.DropIndex(
                name: "IX_DarlingBoost_UsersId",
                table: "DarlingBoost");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "DarlingBoost");

            migrationBuilder.RenameColumn(
                name: "Users_GuildId",
                table: "TempUser",
                newName: "UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_TempUser_Users_GuildId",
                table: "TempUser",
                newName: "IX_TempUser_UsersId");

            migrationBuilder.RenameColumn(
                name: "Users_GuildId",
                table: "Roles_Timer",
                newName: "UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_Timer_Users_GuildId",
                table: "Roles_Timer",
                newName: "IX_Roles_Timer_UsersId");

            migrationBuilder.RenameColumn(
                name: "Users_GuildId",
                table: "PrivateChannels",
                newName: "UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_PrivateChannels_Users_GuildId",
                table: "PrivateChannels",
                newName: "IX_PrivateChannels_UsersId");

            migrationBuilder.RenameColumn(
                name: "Users_GuildId",
                table: "Invites",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Invites_Users_GuildId",
                table: "Invites",
                newName: "IX_Invites_UserId");

            migrationBuilder.AddColumn<bool>(
                name: "BirthDateInvise",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.AddColumn<uint>(
                name: "CountWarns",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<DateTime>(
                name: "Daily",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<ulong>(
                name: "GuildsId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<bool>(
                name: "Leaved",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReportedRules",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "Streak",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "UsersMId",
                table: "Users",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "VoiceActive",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<ulong>(
                name: "XP",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "ZeroCoin",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateIndex(
                name: "IX_Users_BoostId",
                table: "Users",
                column: "BoostId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GuildsId",
                table: "Users",
                column: "GuildsId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UsersMId",
                table: "Users",
                column: "UsersMId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_Users_UserId",
                table: "Invites",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateChannels_Users_UsersId",
                table: "PrivateChannels",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Timer_Users_UsersId",
                table: "Roles_Timer",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TempUser_Users_UsersId",
                table: "TempUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_DarlingBoost_BoostId",
                table: "Users",
                column: "BoostId",
                principalTable: "DarlingBoost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Guilds_GuildsId",
                table: "Users",
                column: "GuildsId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_UsersMId",
                table: "Users",
                column: "UsersMId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
