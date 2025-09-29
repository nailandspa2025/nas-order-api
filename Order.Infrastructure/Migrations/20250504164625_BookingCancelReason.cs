using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BookingCancelReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingCancelReasonId",
                table: "Booking",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookingCancelReason",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingCancelReason", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_BookingCancelReasonId",
                table: "Booking",
                column: "BookingCancelReasonId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_BookingCancelReason_BookingCancelReasonId",
                table: "Booking",
                column: "BookingCancelReasonId",
                principalTable: "BookingCancelReason",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_BookingCancelReason_BookingCancelReasonId",
                table: "Booking");

            migrationBuilder.DropTable(
                name: "BookingCancelReason");

            migrationBuilder.DropIndex(
                name: "IX_Booking_BookingCancelReasonId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "BookingCancelReasonId",
                table: "Booking");
        }
    }
}
