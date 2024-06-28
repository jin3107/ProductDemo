using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductDemo.Data.Migrations
{
    /// <inheritdoc />
    public partial class update_invoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_AspNetUsers_BuyerId",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_BuyerId",
                table: "Invoice");

            migrationBuilder.AlterColumn<string>(
                name: "BuyerId",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BuyerId",
                table: "Invoice",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_BuyerId",
                table: "Invoice",
                column: "BuyerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_AspNetUsers_BuyerId",
                table: "Invoice",
                column: "BuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
