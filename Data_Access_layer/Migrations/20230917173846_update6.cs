using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data_Access_layer.Migrations
{
    /// <inheritdoc />
    public partial class update6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Purchased",
                table: "Shopping");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Shopping",
                newName: "QuantityShopping");

            migrationBuilder.AddColumn<int>(
                name: "QuantityPurchased",
                table: "Shopping",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityPurchased",
                table: "Shopping");

            migrationBuilder.RenameColumn(
                name: "QuantityShopping",
                table: "Shopping",
                newName: "Quantity");

            migrationBuilder.AddColumn<bool>(
                name: "Purchased",
                table: "Shopping",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
