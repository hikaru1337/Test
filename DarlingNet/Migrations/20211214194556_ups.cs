using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class ups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1ul,
                columns: new[] { "Price", "Value" },
                values: new object[] { 800ul, (sbyte)5 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2ul,
                columns: new[] { "Price", "Value" },
                values: new object[] { 5000ul, (sbyte)30 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1ul,
                columns: new[] { "Price", "Value" },
                values: new object[] { 1000ul, (sbyte)15 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2ul,
                columns: new[] { "Price", "Value" },
                values: new object[] { 10000ul, (sbyte)35 });
        }
    }
}
