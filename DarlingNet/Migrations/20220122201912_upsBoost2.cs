using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class upsBoost2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DarlingBoost_Users_UsersId",
                table: "DarlingBoost");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Suspens_SuspensId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "ReportSuspens");

            migrationBuilder.DropTable(
                name: "Suspens");

            migrationBuilder.DropIndex(
                name: "IX_Users_SuspensId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SuspensId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "DarlingBoost",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DarlingBoost_UsersId",
                table: "DarlingBoost",
                newName: "IX_DarlingBoost_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DarlingBoost_Users_UserId",
                table: "DarlingBoost",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DarlingBoost_Users_UserId",
                table: "DarlingBoost");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "DarlingBoost",
                newName: "UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_DarlingBoost_UserId",
                table: "DarlingBoost",
                newName: "IX_DarlingBoost_UsersId");

            migrationBuilder.AddColumn<ulong>(
                name: "SuspensId",
                table: "Users",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Suspens",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suspens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportSuspens",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdministratorId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Automatic = table.Column<bool>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    SuspensId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TypeReport = table.Column<byte>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportSuspens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportSuspens_Suspens_SuspensId",
                        column: x => x.SuspensId,
                        principalTable: "Suspens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportSuspens_Users_AdministratorId",
                        column: x => x.AdministratorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_SuspensId",
                table: "Users",
                column: "SuspensId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSuspens_AdministratorId",
                table: "ReportSuspens",
                column: "AdministratorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSuspens_SuspensId",
                table: "ReportSuspens",
                column: "SuspensId");

            migrationBuilder.AddForeignKey(
                name: "FK_DarlingBoost_Users_UsersId",
                table: "DarlingBoost",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Suspens_SuspensId",
                table: "Users",
                column: "SuspensId",
                principalTable: "Suspens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
