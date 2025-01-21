using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SyncIT.Web.Database.Migrations
{
    /// <inheritdoc />
    public partial class Switchtoarrays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdditionalGroupAliases");

            migrationBuilder.DropTable(
                name: "AdditionalGroupMembers");

            migrationBuilder.DropTable(
                name: "AdditionalUserAliases");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AdditionalUsers",
                newName: "Nick");

            migrationBuilder.AddColumn<string>(
                name: "Aliases",
                table: "AdditionalUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cid",
                table: "AdditionalUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AdditionalUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AdditionalUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecoveryEmail",
                table: "AdditionalUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Aliases",
                table: "AdditionalGroups",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Members",
                table: "AdditionalGroups",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aliases",
                table: "AdditionalUsers");

            migrationBuilder.DropColumn(
                name: "Cid",
                table: "AdditionalUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AdditionalUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AdditionalUsers");

            migrationBuilder.DropColumn(
                name: "RecoveryEmail",
                table: "AdditionalUsers");

            migrationBuilder.DropColumn(
                name: "Aliases",
                table: "AdditionalGroups");

            migrationBuilder.DropColumn(
                name: "Members",
                table: "AdditionalGroups");

            migrationBuilder.RenameColumn(
                name: "Nick",
                table: "AdditionalUsers",
                newName: "Name");

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
    }
}
