using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSystem.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseToSaleItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "SaleItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_WarehouseId",
                table: "SaleItems",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Warehouses_WarehouseId",
                table: "SaleItems",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_Warehouses_WarehouseId",
                table: "SaleItems");

            migrationBuilder.DropIndex(
                name: "IX_SaleItems_WarehouseId",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "SaleItems");
        }
    }
}
