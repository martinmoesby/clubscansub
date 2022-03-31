using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedSpecUsersToEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BoatLeaderId",
                table: "Event",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiveleaderId",
                table: "Event",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StandbyDiverId",
                table: "Event",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_BoatLeaderId",
                table: "Event",
                column: "BoatLeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_DiveleaderId",
                table: "Event",
                column: "DiveleaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_StandbyDiverId",
                table: "Event",
                column: "StandbyDiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_AspNetUsers_BoatLeaderId",
                table: "Event",
                column: "BoatLeaderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_AspNetUsers_DiveleaderId",
                table: "Event",
                column: "DiveleaderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_AspNetUsers_StandbyDiverId",
                table: "Event",
                column: "StandbyDiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_AspNetUsers_BoatLeaderId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_AspNetUsers_DiveleaderId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_AspNetUsers_StandbyDiverId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_BoatLeaderId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_DiveleaderId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_StandbyDiverId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "BoatLeaderId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "DiveleaderId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "StandbyDiverId",
                table: "Event");
        }
    }
}
