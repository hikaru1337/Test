using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class UpdateInvites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pets_UserId",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "DieSms",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Invites");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "Invites",
                newName: "UserId");

            migrationBuilder.AddColumn<ulong>(
                name: "IsBot",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AlterColumn<double>(
                name: "invoice_ammount",
                table: "QiwiTransactions",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Invites",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateIndex(
                name: "IX_Pets_UserId",
                table: "Pets",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invites_UserId",
                table: "Invites",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_Users_UserId",
                table: "Invites",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_Users_UserId",
                table: "Invites");

            migrationBuilder.DropIndex(
                name: "IX_Pets_UserId",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "IX_Invites_UserId",
                table: "Invites");

            migrationBuilder.DropColumn(
                name: "IsBot",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Invites",
                newName: "UsersId");

            migrationBuilder.AlterColumn<ulong>(
                name: "invoice_ammount",
                table: "QiwiTransactions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddColumn<bool>(
                name: "DieSms",
                table: "Pets",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<ulong>(
                name: "Id",
                table: "Invites",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Invites",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pets_UserId",
                table: "Pets",
                column: "UserId");
        }
    }
}
