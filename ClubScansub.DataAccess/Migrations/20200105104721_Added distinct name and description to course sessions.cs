using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class Addeddistinctnameanddescriptiontocoursesessions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionDescription",
                table: "CourseSessions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionName",
                table: "CourseSessions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionDescription",
                table: "CourseSessions");

            migrationBuilder.DropColumn(
                name: "SessionName",
                table: "CourseSessions");
        }
    }
}
