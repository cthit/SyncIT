using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SyncIT.Web.Database.Migrations
{
    /// <inheritdoc />
    public partial class AdditionsId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AdditionalUsers",
                table: "AdditionalUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdditionalGroups",
                table: "AdditionalGroups");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AdditionalUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AdditionalGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdditionalUsers",
                table: "AdditionalUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdditionalGroups",
                table: "AdditionalGroups",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalUsers_Email",
                table: "AdditionalUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalGroups_Email",
                table: "AdditionalGroups",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AdditionalUsers",
                table: "AdditionalUsers");

            migrationBuilder.DropIndex(
                name: "IX_AdditionalUsers_Email",
                table: "AdditionalUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdditionalGroups",
                table: "AdditionalGroups");

            migrationBuilder.DropIndex(
                name: "IX_AdditionalGroups_Email",
                table: "AdditionalGroups");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AdditionalUsers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AdditionalGroups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdditionalUsers",
                table: "AdditionalUsers",
                column: "Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdditionalGroups",
                table: "AdditionalGroups",
                column: "Email");
        }
    }
}
