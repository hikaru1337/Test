using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingDb.Migrations
{
    public partial class ItemPetPriceSbyteToUlong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Emoji", "ItemType", "Name", "PetsId", "Price", "Value" },
                values: new object[] { 1ul, "🍪", 0, "Вискас", null, 1000ul, (sbyte)15 });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Emoji", "ItemType", "Name", "PetsId", "Price", "Value" },
                values: new object[] { 2ul, "🥩", 0, "Мяско", null, 10000ul, (sbyte)35 });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Emoji", "ItemType", "Name", "PetsId", "Price", "Value" },
                values: new object[] { 3ul, "🍗", 0, "Курочка", null, 20000ul, (sbyte)55 });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Emoji", "ItemType", "Name", "PetsId", "Price", "Value" },
                values: new object[] { 4ul, "💊", 1, "Витаминки", null, 10000ul, (sbyte)10 });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Emoji", "ItemType", "Name", "PetsId", "Price", "Value" },
                values: new object[] { 5ul, "🦠", 1, "Антибиотики", null, 20000ul, (sbyte)30 });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Emoji", "ItemType", "Name", "PetsId", "Price", "Value" },
                values: new object[] { 6ul, "💉", 1, "Волшебный шприц", null, 50000ul, (sbyte)65 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1ul);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2ul);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3ul);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4ul);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5ul);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6ul);
        }
    }
}
