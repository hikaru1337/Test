using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DarlingNet.Migrations
{
    public partial class First : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Leaved = table.Column<bool>(type: "INTEGER", nullable: false),
                    Prefix = table.Column<string>(type: "TEXT", nullable: true),
                    ChatMuteRole = table.Column<ulong>(type: "INTEGER", nullable: false),
                    VoiceMuteRole = table.Column<ulong>(type: "INTEGER", nullable: false),
                    PrivateId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    CommandInviseString = table.Column<string>(type: "TEXT", nullable: true),
                    GiveXPnextChannel = table.Column<bool>(type: "INTEGER", nullable: false),
                    VS = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    UseCommand = table.Column<bool>(type: "INTEGER", nullable: false),
                    UseAdminCommand = table.Column<bool>(type: "INTEGER", nullable: false),
                    UseRPcommand = table.Column<bool>(type: "INTEGER", nullable: false),
                    GiveXP = table.Column<bool>(type: "INTEGER", nullable: false),
                    DelUrl = table.Column<bool>(type: "INTEGER", nullable: false),
                    DelUrlImage = table.Column<bool>(type: "INTEGER", nullable: false),
                    DelCaps = table.Column<bool>(type: "INTEGER", nullable: false),
                    Spaming = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendBadWord = table.Column<bool>(type: "INTEGER", nullable: false),
                    BadWordString = table.Column<string>(type: "TEXT", nullable: true),
                    csUrlWhiteListString = table.Column<string>(type: "TEXT", nullable: true),
                    InviteMessage = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Channels_Guilds_GuildsId",
                        column: x => x.GuildsId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Guilds_Raid",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    RaidStop = table.Column<bool>(type: "INTEGER", nullable: false),
                    RaidTime = table.Column<uint>(type: "INTEGER", nullable: false),
                    RaidUserCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    RaidMuted = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds_Raid", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guilds_Raid_Guilds_GuildsId",
                        column: x => x.GuildsId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Guilds_Warns",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    CountWarn = table.Column<byte>(type: "INTEGER", nullable: false),
                    Time = table.Column<string>(type: "TEXT", nullable: true),
                    ReportTypes = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds_Warns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guilds_Warns_Guilds_GuildsId",
                        column: x => x.GuildsId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    GuildsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Value = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Guilds_GuildsId",
                        column: x => x.GuildsId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Leaved = table.Column<bool>(type: "INTEGER", nullable: false),
                    XP = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ZeroCoin = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Daily = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Streak = table.Column<ulong>(type: "INTEGER", nullable: false),
                    CountWarns = table.Column<uint>(type: "INTEGER", nullable: false),
                    UsersMId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Guilds_GuildsId",
                        column: x => x.GuildsId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Users_UsersMId",
                        column: x => x.UsersMId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmoteClick",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Emote = table.Column<string>(type: "TEXT", nullable: true),
                    MessageId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ChannelsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Get = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmoteClick", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmoteClick_Channels_ChannelsId",
                        column: x => x.ChannelsId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Guilds_Logs",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guilds_Logs_Channels_ChannelsId",
                        column: x => x.ChannelsId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Guilds_Meeting_Leave",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    LeaveMessage = table.Column<string>(type: "TEXT", nullable: true),
                    LeaveChannelsId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds_Meeting_Leave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guilds_Meeting_Leave_Channels_LeaveChannelsId",
                        column: x => x.LeaveChannelsId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guilds_Meeting_Leave_Guilds_GuildsId",
                        column: x => x.GuildsId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Guilds_Meeting_Welcome",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WelcomeMessage = table.Column<string>(type: "TEXT", nullable: true),
                    WelcomeDMmessage = table.Column<string>(type: "TEXT", nullable: true),
                    WelcomeDMuser = table.Column<bool>(type: "INTEGER", nullable: false),
                    GuildsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    WelcomeChannelsId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    WelcomeRole = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds_Meeting_Welcome", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guilds_Meeting_Welcome_Channels_WelcomeChannelsId",
                        column: x => x.WelcomeChannelsId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guilds_Meeting_Welcome_Guilds_GuildsId",
                        column: x => x.GuildsId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: true),
                    Times = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Repeat = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Channels_ChannelsId",
                        column: x => x.ChannelsId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DarlingBoost",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsersId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Streak = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Ends = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DarlingBoost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DarlingBoost_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invites",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: true),
                    UsersId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    UsesCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invites_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrivateChannels",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VoiceChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivateChannels_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TempUser",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ToTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Reason = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TempUser_Channels_ChannelsId",
                        column: x => x.ChannelsId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TempUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Channels_GuildsId",
                table: "Channels",
                column: "GuildsId");

            migrationBuilder.CreateIndex(
                name: "IX_DarlingBoost_UsersId",
                table: "DarlingBoost",
                column: "UsersId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmoteClick_ChannelsId",
                table: "EmoteClick",
                column: "ChannelsId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_Logs_ChannelsId",
                table: "Guilds_Logs",
                column: "ChannelsId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_Meeting_Leave_GuildsId",
                table: "Guilds_Meeting_Leave",
                column: "GuildsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_Meeting_Leave_LeaveChannelsId",
                table: "Guilds_Meeting_Leave",
                column: "LeaveChannelsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_Meeting_Welcome_GuildsId",
                table: "Guilds_Meeting_Welcome",
                column: "GuildsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_Meeting_Welcome_WelcomeChannelsId",
                table: "Guilds_Meeting_Welcome",
                column: "WelcomeChannelsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_Raid_GuildsId",
                table: "Guilds_Raid",
                column: "GuildsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_Warns_GuildsId",
                table: "Guilds_Warns",
                column: "GuildsId");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_UsersId",
                table: "Invites",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateChannels_UsersId",
                table: "PrivateChannels",
                column: "UsersId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_GuildsId",
                table: "Roles",
                column: "GuildsId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ChannelsId",
                table: "Tasks",
                column: "ChannelsId");

            migrationBuilder.CreateIndex(
                name: "IX_TempUser_ChannelsId",
                table: "TempUser",
                column: "ChannelsId");

            migrationBuilder.CreateIndex(
                name: "IX_TempUser_UsersId",
                table: "TempUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GuildsId",
                table: "Users",
                column: "GuildsId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UsersMId",
                table: "Users",
                column: "UsersMId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DarlingBoost");

            migrationBuilder.DropTable(
                name: "EmoteClick");

            migrationBuilder.DropTable(
                name: "Guilds_Logs");

            migrationBuilder.DropTable(
                name: "Guilds_Meeting_Leave");

            migrationBuilder.DropTable(
                name: "Guilds_Meeting_Welcome");

            migrationBuilder.DropTable(
                name: "Guilds_Raid");

            migrationBuilder.DropTable(
                name: "Guilds_Warns");

            migrationBuilder.DropTable(
                name: "Invites");

            migrationBuilder.DropTable(
                name: "PrivateChannels");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "TempUser");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Guilds");
        }
    }
}
