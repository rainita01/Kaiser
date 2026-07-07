using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kaiser.Migrations
{
    /// <inheritdoc />
    public partial class initOrderToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Payments",
                newName: "RefId");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "Payments",
                newName: "Authority");

            migrationBuilder.AddColumn<string>(
                name: "Authorize",
                table: "SnapShots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "SnapShots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "GatewayStatusCode",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SnapShotId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "OrderItems",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SnapShotId",
                table: "Payments",
                column: "SnapShotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentId",
                table: "Orders",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_SnapShots_SnapShotId",
                table: "Payments",
                column: "SnapShotId",
                principalTable: "SnapShots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_SnapShots_SnapShotId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_SnapShotId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Authorize",
                table: "SnapShots");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "SnapShots");

            migrationBuilder.DropColumn(
                name: "GatewayStatusCode",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SnapShotId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "RefId",
                table: "Payments",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "Authority",
                table: "Payments",
                newName: "PaymentMethod");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentId",
                table: "Orders",
                column: "PaymentId",
                unique: true);
        }
    }
}
