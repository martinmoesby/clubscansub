using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubScansub.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedEventTONavPropertyOnEventRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventRequests_Event_EventId",
                table: "EventRequests");

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "EventRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventRequests_Event_EventId",
                table: "EventRequests",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventRequests_Event_EventId",
                table: "EventRequests");

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "EventRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRequests_Event_EventId",
                table: "EventRequests",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");
        }
    }
}
