using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SyncIT.Web.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLastUsedTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedAsSource",
                table: "BaseSyncServiceConfigs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedAsTarget",
                table: "BaseSyncServiceConfigs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUsedAsSource",
                table: "BaseSyncServiceConfigs");

            migrationBuilder.DropColumn(
                name: "LastUsedAsTarget",
                table: "BaseSyncServiceConfigs");
        }
    }
}
