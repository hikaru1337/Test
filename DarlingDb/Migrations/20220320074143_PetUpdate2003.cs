using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class PetUpdate2003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HP",
                table: "Pets",
                newName: "XP");

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1337ul,
                columns: new[] { "EAT", "MOOD", "SLEEP", "XP" },
                values: new object[] { (byte)0, (byte)0, (byte)0, 0u });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "XP",
                table: "Pets",
                newName: "HP");

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1337ul,
                columns: new[] { "EAT", "HP", "MOOD", "SLEEP" },
                values: new object[] { (sbyte)0, (sbyte)0, (sbyte)0, (sbyte)0 });
        }
    }
}
