using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class PropertiesToShopFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShopId",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_ShopId",
                table: "Properties",
                column: "ShopId");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Shops_ShopId",
                table: "Properties",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Shops_ShopId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_ShopId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "Properties");
        }
    }
}
