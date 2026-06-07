using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kaiser.Migrations
{
    /// <inheritdoc />
    public partial class adddiscountPercent_Isbestsell_product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "Images");

            migrationBuilder.AddColumn<int>(
                name: "DiscountPercent",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsBestSell",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsBestSell",
                table: "Products");

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "Images",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
