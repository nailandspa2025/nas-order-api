using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PaymentUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Payment",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Payment");
        }
    }
}
