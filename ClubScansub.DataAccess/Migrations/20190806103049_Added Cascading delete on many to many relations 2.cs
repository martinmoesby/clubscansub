using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedCascadingdeleteonmanytomanyrelations2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_AspNetUsers_ApplicationUserId",
                table: "EventUser");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "EventUser",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventUser_ApplicationUserId1",
                table: "EventUser",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_AspNetUsers_ApplicationUserId1",
                table: "EventUser",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_AspNetUsers_ApplicationUserId1",
                table: "EventUser");

            migrationBuilder.DropIndex(
                name: "IX_EventUser_ApplicationUserId1",
                table: "EventUser");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "EventUser");

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_AspNetUsers_ApplicationUserId",
                table: "EventUser",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
