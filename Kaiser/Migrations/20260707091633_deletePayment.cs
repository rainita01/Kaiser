using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kaiser.Migrations
{
    /// <inheritdoc />
    public partial class deletePayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropColumn(
                name: "Authorize",
                table: "SnapShots");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "SnapShots");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Orders",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Orders",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "OrderTime",
                table: "Orders",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "OrderItems",
                newName: "Count");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<long>(
                name: "ShippingCost",
                table: "Orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "SnapShotId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SnapShotId",
                table: "Orders",
                column: "SnapShotId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_SnapShots_SnapShotId",
                table: "Orders",
                column: "SnapShotId",
                principalTable: "SnapShots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_SnapShots_SnapShotId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_SnapShotId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingCost",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SnapShotId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Orders",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Orders",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Orders",
                newName: "OrderTime");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "OrderItems",
                newName: "Quantity");

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

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SnapShotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Authority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_SnapShots_SnapShotId",
                        column: x => x.SnapShotId,
                        principalTable: "SnapShots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    AdditionalData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    CardHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardPan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fee = table.Column<long>(type: "bigint", nullable: true),
                    FeeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefId = table.Column<long>(type: "bigint", nullable: true),
                    Succeed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SnapShotId",
                table: "Payments",
                column: "SnapShotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PaymentId",
                table: "Transactions",
                column: "PaymentId");
        }
    }
}
