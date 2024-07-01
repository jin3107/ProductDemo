using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductDemo.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatecartitem2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorOption",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorOption",
                table: "CartItem");
        }
    }
}
