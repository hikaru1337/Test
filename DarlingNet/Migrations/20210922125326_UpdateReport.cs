using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class UpdateReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "Reports_Punishes");

            migrationBuilder.AddColumn<string>(
                name: "ReportedRules",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportedRules",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Reports_Punishes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
