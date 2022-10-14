using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class UpdateUserAddVoiceActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "VoiceActive",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoiceActive",
                table: "Users");
        }
    }
}
