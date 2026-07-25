using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSystem.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseToPurchaseItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "PurchaseItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_WarehouseId",
                table: "PurchaseItems",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Warehouses_WarehouseId",
                table: "PurchaseItems",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Warehouses_WarehouseId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_WarehouseId",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "PurchaseItems");
        }
    }
}
