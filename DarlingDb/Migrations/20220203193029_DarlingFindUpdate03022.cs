using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingDb.Migrations
{
    public partial class DarlingFindUpdate03022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Darling_Reports");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Darling");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Darling_Reports",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Darling",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Darling_Reports");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Darling");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Darling_Reports",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Darling",
                type: "BLOB",
                nullable: true);
        }
    }
}
