using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Snap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingSnap_Booking_BookingId1",
                table: "BookingSnap");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingSnapGroup_Booking_BookingId1",
                table: "BookingSnapGroup");

            migrationBuilder.DropIndex(
                name: "IX_BookingSnapGroup_BookingId1",
                table: "BookingSnapGroup");

            migrationBuilder.DropIndex(
                name: "IX_BookingSnap_BookingId1",
                table: "BookingSnap");

            migrationBuilder.DropColumn(
                name: "BookingId1",
                table: "BookingSnapGroup");

            migrationBuilder.DropColumn(
                name: "BookingId1",
                table: "BookingSnap");

            migrationBuilder.AlterColumn<int>(
                name: "BookingId",
                table: "BookingSnapGroup",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "BookingId",
                table: "BookingSnap",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSnapGroup_BookingId",
                table: "BookingSnapGroup",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSnap_BookingId",
                table: "BookingSnap",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSnap_Booking_BookingId",
                table: "BookingSnap",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSnapGroup_Booking_BookingId",
                table: "BookingSnapGroup",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingSnap_Booking_BookingId",
                table: "BookingSnap");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingSnapGroup_Booking_BookingId",
                table: "BookingSnapGroup");

            migrationBuilder.DropIndex(
                name: "IX_BookingSnapGroup_BookingId",
                table: "BookingSnapGroup");

            migrationBuilder.DropIndex(
                name: "IX_BookingSnap_BookingId",
                table: "BookingSnap");

            migrationBuilder.AlterColumn<long>(
                name: "BookingId",
                table: "BookingSnapGroup",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "BookingId1",
                table: "BookingSnapGroup",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "BookingId",
                table: "BookingSnap",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "BookingId1",
                table: "BookingSnap",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BookingSnapGroup_BookingId1",
                table: "BookingSnapGroup",
                column: "BookingId1");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSnap_BookingId1",
                table: "BookingSnap",
                column: "BookingId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSnap_Booking_BookingId1",
                table: "BookingSnap",
                column: "BookingId1",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSnapGroup_Booking_BookingId1",
                table: "BookingSnapGroup",
                column: "BookingId1",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
