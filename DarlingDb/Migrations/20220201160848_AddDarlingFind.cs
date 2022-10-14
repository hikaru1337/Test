using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingDb.Migrations
{
    public partial class AddDarlingFind : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Darling",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsersId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Year = table.Column<sbyte>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Sex = table.Column<int>(type: "INTEGER", nullable: false),
                    SexSearch = table.Column<int>(type: "INTEGER", nullable: false),
                    AcceptRules = table.Column<bool>(type: "INTEGER", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Visible = table.Column<bool>(type: "INTEGER", nullable: false),
                    Blocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    DarlingLastId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Darling", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Darling_Darling_DarlingLastId",
                        column: x => x.DarlingLastId,
                        principalTable: "Darling",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Darling_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Darling_Hobbies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Darling_Hobbies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Darling_LikeDisLike",
                columns: table => new
                {
                    FirstDarlingId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    SecondDarlingId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ThisState = table.Column<int>(type: "INTEGER", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Darling_LikeDisLike", x => new { x.FirstDarlingId, x.SecondDarlingId });
                    table.ForeignKey(
                        name: "FK_Darling_LikeDisLike_Darling_FirstDarlingId",
                        column: x => x.FirstDarlingId,
                        principalTable: "Darling",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Darling_LikeDisLike_Darling_SecondDarlingId",
                        column: x => x.SecondDarlingId,
                        principalTable: "Darling",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Darling_Reports",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DarlingId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Darling_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Darling_Reports_Darling_DarlingId",
                        column: x => x.DarlingId,
                        principalTable: "Darling",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DarlingDarling_Hobbies",
                columns: table => new
                {
                    DarlingId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    HobbiesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DarlingDarling_Hobbies", x => new { x.DarlingId, x.HobbiesId });
                    table.ForeignKey(
                        name: "FK_DarlingDarling_Hobbies_Darling_DarlingId",
                        column: x => x.DarlingId,
                        principalTable: "Darling",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DarlingDarling_Hobbies_Darling_Hobbies_HobbiesId",
                        column: x => x.HobbiesId,
                        principalTable: "Darling_Hobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Знакомства" });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "CS" });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Minecraft" });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "Valorant" });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 5, "Lol" });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 6, "Fortnite" });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 7, "Gta5" });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 8, "Dota2" });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 9, "Cod" });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 10, "Wow" });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 11, "Wot" });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 12, "Overwatch" });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 13, "HeartStone" });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 14, "BattleField" });

            migrationBuilder.InsertData(
                table: "Darling_Hobbies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 15, "OSU" });

            migrationBuilder.CreateIndex(
                name: "IX_Darling_DarlingLastId",
                table: "Darling",
                column: "DarlingLastId");

            migrationBuilder.CreateIndex(
                name: "IX_Darling_UsersId",
                table: "Darling",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Darling_LikeDisLike_SecondDarlingId",
                table: "Darling_LikeDisLike",
                column: "SecondDarlingId");

            migrationBuilder.CreateIndex(
                name: "IX_Darling_Reports_DarlingId",
                table: "Darling_Reports",
                column: "DarlingId");

            migrationBuilder.CreateIndex(
                name: "IX_DarlingDarling_Hobbies_HobbiesId",
                table: "DarlingDarling_Hobbies",
                column: "HobbiesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Darling_LikeDisLike");

            migrationBuilder.DropTable(
                name: "Darling_Reports");

            migrationBuilder.DropTable(
                name: "DarlingDarling_Hobbies");

            migrationBuilder.DropTable(
                name: "Darling");

            migrationBuilder.DropTable(
                name: "Darling_Hobbies");
        }
    }
}
