using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SyncIT.Web.Database.Migrations
{
    /// <inheritdoc />
    public partial class GSuiteNewAccountEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SendNewUserEmail",
                table: "BaseSyncServiceConfigs",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendNewUserEmail",
                table: "BaseSyncServiceConfigs");
        }
    }
}
