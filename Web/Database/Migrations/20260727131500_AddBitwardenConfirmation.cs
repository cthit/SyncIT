using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SyncIT.Web.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddBitwardenConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "BitwardenInstances",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BwServeUrl",
                table: "BitwardenInstances",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastConfirmDate",
                table: "BitwardenInstances",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastConfirmCount",
                table: "BitwardenInstances",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "BitwardenInstances");

            migrationBuilder.DropColumn(
                name: "BwServeUrl",
                table: "BitwardenInstances");

            migrationBuilder.DropColumn(
                name: "LastConfirmDate",
                table: "BitwardenInstances");

            migrationBuilder.DropColumn(
                name: "LastConfirmCount",
                table: "BitwardenInstances");
        }
    }
}
