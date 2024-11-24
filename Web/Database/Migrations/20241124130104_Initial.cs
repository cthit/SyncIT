using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SyncIT.Web.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdditionalGroups",
                columns: table => new
                {
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalGroups", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "AdditionalUsers",
                columns: table => new
                {
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalUsers", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "BaseSyncServiceConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    AuthJson = table.Column<string>(type: "TEXT", nullable: true),
                    AdminEmail = table.Column<string>(type: "TEXT", nullable: true),
                    IsReadOnly = table.Column<bool>(type: "INTEGER", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    Token = table.Column<string>(type: "TEXT", nullable: true),
                    EmailDomain = table.Column<string>(type: "TEXT", nullable: true),
                    JsonServiceConfig_IsReadOnly = table.Column<bool>(type: "INTEGER", nullable: true),
                    FilePath = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseSyncServiceConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdditionalGroupAliases",
                columns: table => new
                {
                    GroupEmail = table.Column<string>(type: "TEXT", nullable: false),
                    AliasEmail = table.Column<string>(type: "TEXT", nullable: false),
                    AdditionalGroupEmail = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalGroupAliases", x => new { x.GroupEmail, x.AliasEmail });
                    table.ForeignKey(
                        name: "FK_AdditionalGroupAliases_AdditionalGroups_AdditionalGroupEmail",
                        column: x => x.AdditionalGroupEmail,
                        principalTable: "AdditionalGroups",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "AdditionalGroupMembers",
                columns: table => new
                {
                    GroupEmail = table.Column<string>(type: "TEXT", nullable: false),
                    MemberEmail = table.Column<string>(type: "TEXT", nullable: false),
                    AdditionalGroupEmail = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalGroupMembers", x => new { x.GroupEmail, x.MemberEmail });
                    table.ForeignKey(
                        name: "FK_AdditionalGroupMembers_AdditionalGroups_AdditionalGroupEmail",
                        column: x => x.AdditionalGroupEmail,
                        principalTable: "AdditionalGroups",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "AdditionalUserAliases",
                columns: table => new
                {
                    UserEmail = table.Column<string>(type: "TEXT", nullable: false),
                    AliasEmail = table.Column<string>(type: "TEXT", nullable: false),
                    AdditionalUserEmail = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalUserAliases", x => new { x.UserEmail, x.AliasEmail });
                    table.ForeignKey(
                        name: "FK_AdditionalUserAliases_AdditionalUsers_AdditionalUserEmail",
                        column: x => x.AdditionalUserEmail,
                        principalTable: "AdditionalUsers",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalGroupAliases_AdditionalGroupEmail",
                table: "AdditionalGroupAliases",
                column: "AdditionalGroupEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalGroupMembers_AdditionalGroupEmail",
                table: "AdditionalGroupMembers",
                column: "AdditionalGroupEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalUserAliases_AdditionalUserEmail",
                table: "AdditionalUserAliases",
                column: "AdditionalUserEmail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdditionalGroupAliases");

            migrationBuilder.DropTable(
                name: "AdditionalGroupMembers");

            migrationBuilder.DropTable(
                name: "AdditionalUserAliases");

            migrationBuilder.DropTable(
                name: "BaseSyncServiceConfigs");

            migrationBuilder.DropTable(
                name: "AdditionalGroups");

            migrationBuilder.DropTable(
                name: "AdditionalUsers");
        }
    }
}
