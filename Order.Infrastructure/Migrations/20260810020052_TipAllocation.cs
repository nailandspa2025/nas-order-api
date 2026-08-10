using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TipAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "TipAllocation");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "TipAllocation");

            migrationBuilder.DropColumn(
                name: "Ratio",
                table: "TipAllocation");

            migrationBuilder.RenameColumn(
                name: "ServiceAmount",
                table: "TipAllocation",
                newName: "TechnicianRevenue");

            migrationBuilder.AddColumn<int>(
                name: "AllocationType",
                table: "TipAllocation",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Percentage",
                table: "Payment",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TipAmount",
                table: "Payment",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipType",
                table: "Payment",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllocationType",
                table: "TipAllocation");

            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "TipAmount",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "TipType",
                table: "Payment");

            migrationBuilder.RenameColumn(
                name: "TechnicianRevenue",
                table: "TipAllocation",
                newName: "ServiceAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "TipAllocation",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "TipAllocation",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Ratio",
                table: "TipAllocation",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
