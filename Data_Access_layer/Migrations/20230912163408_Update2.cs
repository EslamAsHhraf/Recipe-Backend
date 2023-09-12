using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data_Access_layer.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "RecipeIngredients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Quantity",
                table: "RecipeIngredients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
