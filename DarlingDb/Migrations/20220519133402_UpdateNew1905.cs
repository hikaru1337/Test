using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class UpdateNew1905 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelId",
                table: "Guilds_Captcha");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Captcha_Role_RoleId",
                table: "Guilds_Captcha");

            migrationBuilder.DropTable(
                name: "Darling_LikeDisLike");

            migrationBuilder.DropTable(
                name: "DarlingDarling_Hobbies");

            migrationBuilder.DropTable(
                name: "Darling");

            migrationBuilder.DropTable(
                name: "Darling_Hobbies");

            migrationBuilder.DropTable(
                name: "Darling_Reports");

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

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1337ul);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReputation",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<ulong>(
                name: "LastReputationUserId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "Reputation",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelId",
                table: "Guilds_Captcha",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Captcha_Role_RoleId",
                table: "Guilds_Captcha",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelId",
                table: "Guilds_Captcha");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Captcha_Role_RoleId",
                table: "Guilds_Captcha");

            migrationBuilder.DropColumn(
                name: "LastReputation",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastReputationUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Reputation",
                table: "Users");

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
                name: "Darling_Reports",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ReportTrue = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Darling_Reports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Darling",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AcceptRules = table.Column<bool>(type: "INTEGER", nullable: false),
                    Blocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    DarlingLastId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    LastMessage = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ReportLastId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    ReportPermission = table.Column<bool>(type: "INTEGER", nullable: false),
                    Sex = table.Column<int>(type: "INTEGER", nullable: false),
                    SexSearch = table.Column<int>(type: "INTEGER", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Visible = table.Column<bool>(type: "INTEGER", nullable: false),
                    Year = table.Column<sbyte>(type: "INTEGER", nullable: false)
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
                        name: "FK_Darling_Darling_Reports_ReportLastId",
                        column: x => x.ReportLastId,
                        principalTable: "Darling_Reports",
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

            migrationBuilder.InsertData(
                table: "Pets",
                columns: new[] { "Id", "DateOfBirth", "Die", "EAT", "LastEat", "LastMood", "LastSleep", "MOOD", "Name", "PetType", "RegenController", "SLEEP", "SleepNow", "UserId", "XP" },
                values: new object[] { 1337ul, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, (byte)0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)0, null, (byte)0, false, (byte)0, false, 663381953181122570ul, 0u });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Emoji", "ItemType", "Name", "PetsId", "Price", "Value" },
                values: new object[] { 1ul, "🍪", (byte)0, "Вискас", 1337ul, 800ul, (sbyte)5 });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Emoji", "ItemType", "Name", "PetsId", "Price", "Value" },
                values: new object[] { 2ul, "🥩", (byte)0, "Мяско", 1337ul, 5000ul, (sbyte)30 });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Emoji", "ItemType", "Name", "PetsId", "Price", "Value" },
                values: new object[] { 3ul, "🍗", (byte)0, "Курочка", 1337ul, 20000ul, (sbyte)55 });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Emoji", "ItemType", "Name", "PetsId", "Price", "Value" },
                values: new object[] { 4ul, "💊", (byte)1, "Витаминки", 1337ul, 10000ul, (sbyte)10 });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Emoji", "ItemType", "Name", "PetsId", "Price", "Value" },
                values: new object[] { 5ul, "🦠", (byte)1, "Антибиотики", 1337ul, 20000ul, (sbyte)30 });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Emoji", "ItemType", "Name", "PetsId", "Price", "Value" },
                values: new object[] { 6ul, "💉", (byte)1, "Волшебный шприц", 1337ul, 50000ul, (sbyte)65 });

            migrationBuilder.CreateIndex(
                name: "IX_Darling_DarlingLastId",
                table: "Darling",
                column: "DarlingLastId");

            migrationBuilder.CreateIndex(
                name: "IX_Darling_ReportLastId",
                table: "Darling",
                column: "ReportLastId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Darling_UsersId",
                table: "Darling",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Darling_LikeDisLike_SecondDarlingId",
                table: "Darling_LikeDisLike",
                column: "SecondDarlingId");

            migrationBuilder.CreateIndex(
                name: "IX_DarlingDarling_Hobbies_HobbiesId",
                table: "DarlingDarling_Hobbies",
                column: "HobbiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Captcha_Channels_ChannelId",
                table: "Guilds_Captcha",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Captcha_Role_RoleId",
                table: "Guilds_Captcha",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
