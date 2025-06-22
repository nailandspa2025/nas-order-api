using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditNotifyBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SentTime",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Notification");

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Notification",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Notification",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_BookingId",
                table: "Notification",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Booking_BookingId",
                table: "Notification",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Booking_BookingId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_BookingId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Notification");

            migrationBuilder.AddColumn<DateTime>(
                name: "SentTime",
                table: "Notification",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Notification",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
