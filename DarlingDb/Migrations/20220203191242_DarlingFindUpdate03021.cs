using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingDb.Migrations
{
    public partial class DarlingFindUpdate03021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Darling_Reports_Darling_DarlingId",
                table: "Darling_Reports");

            migrationBuilder.DropIndex(
                name: "IX_Darling_Reports_DarlingId",
                table: "Darling_Reports");

            migrationBuilder.RenameColumn(
                name: "DarlingId",
                table: "Darling_Reports",
                newName: "ReportTrue");

            migrationBuilder.AddColumn<string>(
                name: "LastMessage",
                table: "Darling",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "ReportLastId",
                table: "Darling",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Darling_ReportLastId",
                table: "Darling",
                column: "ReportLastId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Darling_Darling_Reports_ReportLastId",
                table: "Darling",
                column: "ReportLastId",
                principalTable: "Darling_Reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Darling_Darling_Reports_ReportLastId",
                table: "Darling");

            migrationBuilder.DropIndex(
                name: "IX_Darling_ReportLastId",
                table: "Darling");

            migrationBuilder.DropColumn(
                name: "LastMessage",
                table: "Darling");

            migrationBuilder.DropColumn(
                name: "ReportLastId",
                table: "Darling");

            migrationBuilder.RenameColumn(
                name: "ReportTrue",
                table: "Darling_Reports",
                newName: "DarlingId");

            migrationBuilder.CreateIndex(
                name: "IX_Darling_Reports_DarlingId",
                table: "Darling_Reports",
                column: "DarlingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Darling_Reports_Darling_DarlingId",
                table: "Darling_Reports",
                column: "DarlingId",
                principalTable: "Darling",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
