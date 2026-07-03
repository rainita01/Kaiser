using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kaiser.Migrations
{
    /// <inheritdoc />
    public partial class removefieldssnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "SnapShots");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "SnapShots");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "SnapShots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Discount",
                table: "SnapShots",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "SnapShots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "SubTotal",
                table: "SnapShots",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
