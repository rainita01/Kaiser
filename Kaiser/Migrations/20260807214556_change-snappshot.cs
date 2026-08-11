using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kaiser.Migrations
{
    /// <inheritdoc />
    public partial class changesnappshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_SnapShots_SnapShotId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_SnapShotId",
                table: "Payments");

            migrationBuilder.AddColumn<string>(
                name: "Authority",
                table: "SnapShots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SnapShotId",
                table: "Payments",
                column: "SnapShotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_SnapShots_SnapShotId",
                table: "Payments",
                column: "SnapShotId",
                principalTable: "SnapShots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.DropColumn(
                name: "Authority",
                table: "SnapShots");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SnapShotId",
                table: "Payments",
                column: "SnapShotId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_SnapShots_SnapShotId",
                table: "Payments",
                column: "SnapShotId",
                principalTable: "SnapShots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
