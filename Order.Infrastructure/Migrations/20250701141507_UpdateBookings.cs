using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Booking_BookingCancelReasonId",
                table: "Booking");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_BookingCancelReasonId",
                table: "Booking",
                column: "BookingCancelReasonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Booking_BookingCancelReasonId",
                table: "Booking");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_BookingCancelReasonId",
                table: "Booking",
                column: "BookingCancelReasonId",
                unique: true);
        }
    }
}
