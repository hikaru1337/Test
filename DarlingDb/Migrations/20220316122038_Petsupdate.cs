using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class Petsupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Pets_PetsId",
                table: "Items");

            migrationBuilder.AlterColumn<ulong>(
                name: "PetsId",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Pets",
                columns: new[] { "Id", "DateOfBirth", "Die", "EAT", "HP", "LastEat", "LastMood", "LastSleep", "MOOD", "Name", "PetType", "RegenController", "SLEEP", "SleepNow", "UserId" },
                values: new object[] { 1337ul, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, (sbyte)0, (sbyte)0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), (sbyte)0, null, (byte)0, false, (sbyte)0, false, 663381953181122570ul });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1ul,
                column: "PetsId",
                value: 1337ul);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2ul,
                column: "PetsId",
                value: 1337ul);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3ul,
                column: "PetsId",
                value: 1337ul);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4ul,
                column: "PetsId",
                value: 1337ul);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5ul,
                column: "PetsId",
                value: 1337ul);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6ul,
                column: "PetsId",
                value: 1337ul);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Pets_PetsId",
                table: "Items",
                column: "PetsId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Pets_PetsId",
                table: "Items");

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1337ul);

            migrationBuilder.AlterColumn<ulong>(
                name: "PetsId",
                table: "Items",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1ul,
                column: "PetsId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2ul,
                column: "PetsId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3ul,
                column: "PetsId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4ul,
                column: "PetsId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5ul,
                column: "PetsId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6ul,
                column: "PetsId",
                value: null);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Pets_PetsId",
                table: "Items",
                column: "PetsId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
