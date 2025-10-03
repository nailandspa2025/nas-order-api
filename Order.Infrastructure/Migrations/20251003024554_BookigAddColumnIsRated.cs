using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BookigAddColumnIsRated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRated",
                table: "Booking",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRated",
                table: "Booking");
        }
    }
}
