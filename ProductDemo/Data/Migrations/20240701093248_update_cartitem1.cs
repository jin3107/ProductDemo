using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductDemo.Data.Migrations
{
    /// <inheritdoc />
    public partial class update_cartitem1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardOption",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Configuration",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descriptions",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WindowsOption",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardOption",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "Configuration",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "Descriptions",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "WindowsOption",
                table: "CartItem");
        }
    }
}
