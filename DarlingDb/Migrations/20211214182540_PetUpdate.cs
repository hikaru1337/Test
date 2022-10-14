using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingDb.Migrations
{
    public partial class PetUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    RegenController = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastEat = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastMood = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastSleep = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SleepNow = table.Column<bool>(type: "INTEGER", nullable: false),
                    PetType = table.Column<int>(type: "INTEGER", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HP = table.Column<sbyte>(type: "INTEGER", nullable: false),
                    MOOD = table.Column<sbyte>(type: "INTEGER", nullable: false),
                    EAT = table.Column<sbyte>(type: "INTEGER", nullable: false),
                    SLEEP = table.Column<sbyte>(type: "INTEGER", nullable: false),
                    Die = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Emoji = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    PetsId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Price = table.Column<sbyte>(type: "INTEGER", nullable: false),
                    Value = table.Column<sbyte>(type: "INTEGER", nullable: false),
                    ItemType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Pets_PetsId",
                        column: x => x.PetsId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_PetsId",
                table: "Items",
                column: "PetsId");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_UserId",
                table: "Pets",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Pets");
        }
    }
}
