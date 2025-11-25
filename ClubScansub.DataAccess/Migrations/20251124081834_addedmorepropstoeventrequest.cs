using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace ClubScansub.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedmorepropstoeventrequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "EventRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequestApproved",
                table: "EventRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequestProcessed",
                table: "EventRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_EventRequests_EventId",
                table: "EventRequests",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRequests_Event_EventId",
                table: "EventRequests",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventRequests_Event_EventId",
                table: "EventRequests");

            migrationBuilder.DropIndex(
                name: "IX_EventRequests_EventId",
                table: "EventRequests");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "EventRequests");

            migrationBuilder.DropColumn(
                name: "RequestApproved",
                table: "EventRequests");

            migrationBuilder.DropColumn(
                name: "RequestProcessed",
                table: "EventRequests");
        }
    }
}
