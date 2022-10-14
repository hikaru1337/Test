using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class Roles_TimeAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Roles",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.CreateTable(
                name: "Roles_Timer",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    GuildsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ToTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles_Timer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Timer_Guilds_GuildsId",
                        column: x => x.GuildsId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Roles_Timer_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Timer_GuildsId",
                table: "Roles_Timer",
                column: "GuildsId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Timer_UsersId",
                table: "Roles_Timer",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Roles_Timer");

            migrationBuilder.AlterColumn<ulong>(
                name: "Value",
                table: "Roles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
