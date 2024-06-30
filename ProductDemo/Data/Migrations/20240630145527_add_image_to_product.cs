using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductDemo.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_image_to_product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Product",
                newName: "ImagePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Product",
                newName: "Image");
        }
    }
}
