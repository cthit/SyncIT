using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SyncIT.Web.Database.Migrations
{
    /// <inheritdoc />
    public partial class OverwriteToggle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OverwriteCid",
                table: "AdditionalUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OverwriteFirstName",
                table: "AdditionalUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OverwriteLastName",
                table: "AdditionalUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OverwriteNick",
                table: "AdditionalUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OverwriteRecoveryEmail",
                table: "AdditionalUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OverwriteName",
                table: "AdditionalGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverwriteCid",
                table: "AdditionalUsers");

            migrationBuilder.DropColumn(
                name: "OverwriteFirstName",
                table: "AdditionalUsers");

            migrationBuilder.DropColumn(
                name: "OverwriteLastName",
                table: "AdditionalUsers");

            migrationBuilder.DropColumn(
                name: "OverwriteNick",
                table: "AdditionalUsers");

            migrationBuilder.DropColumn(
                name: "OverwriteRecoveryEmail",
                table: "AdditionalUsers");

            migrationBuilder.DropColumn(
                name: "OverwriteName",
                table: "AdditionalGroups");
        }
    }
}
