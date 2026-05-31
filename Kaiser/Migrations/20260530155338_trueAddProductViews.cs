using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kaiser.Migrations
{
    /// <inheritdoc />
    public partial class trueAddProductViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductView_Products_ProductId",
                table: "ProductView");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductView",
                table: "ProductView");

            migrationBuilder.RenameTable(
                name: "ProductView",
                newName: "ProductViews");

            migrationBuilder.RenameIndex(
                name: "IX_ProductView_ProductId",
                table: "ProductViews",
                newName: "IX_ProductViews_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductViews",
                table: "ProductViews",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductViews_Products_ProductId",
                table: "ProductViews",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductViews_Products_ProductId",
                table: "ProductViews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductViews",
                table: "ProductViews");

            migrationBuilder.RenameTable(
                name: "ProductViews",
                newName: "ProductView");

            migrationBuilder.RenameIndex(
                name: "IX_ProductViews_ProductId",
                table: "ProductView",
                newName: "IX_ProductView_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductView",
                table: "ProductView",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductView_Products_ProductId",
                table: "ProductView",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
