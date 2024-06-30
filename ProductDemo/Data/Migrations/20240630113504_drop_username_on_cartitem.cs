using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductDemo.Data.Migrations
{
    /// <inheritdoc />
    public partial class drop_username_on_cartitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "CartItem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
