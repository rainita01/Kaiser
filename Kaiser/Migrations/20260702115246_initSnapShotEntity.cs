using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kaiser.Migrations
{
    /// <inheritdoc />
    public partial class initSnapShotEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SnapShots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubTotal = table.Column<long>(type: "bigint", nullable: false),
                    Discount = table.Column<long>(type: "bigint", nullable: false),
                    ShippingCost = table.Column<long>(type: "bigint", nullable: false),
                    TotalPrice = table.Column<long>(type: "bigint", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnapShots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SnapShots_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SnapShots_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SnapShotItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitPrice = table.Column<long>(type: "bigint", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<long>(type: "bigint", nullable: false),
                    SnapShotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnapShotItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SnapShotItems_SnapShots_SnapShotId",
                        column: x => x.SnapShotId,
                        principalTable: "SnapShots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SnapShotItems_SnapShotId",
                table: "SnapShotItems",
                column: "SnapShotId");

            migrationBuilder.CreateIndex(
                name: "IX_SnapShots_AddressId",
                table: "SnapShots",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_SnapShots_UserId",
                table: "SnapShots",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SnapShotItems");

            migrationBuilder.DropTable(
                name: "SnapShots");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Payments");
        }
    }
}
