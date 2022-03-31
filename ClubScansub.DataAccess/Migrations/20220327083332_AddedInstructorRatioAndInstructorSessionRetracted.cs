using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedInstructorRatioAndInstructorSessionRetracted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstructorRatio",
                table: "CourseSessionTemplate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "InstructorRetracted",
                table: "CourseSessionInstructor",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstructorRatio",
                table: "CourseSessionTemplate");

            migrationBuilder.DropColumn(
                name: "InstructorRetracted",
                table: "CourseSessionInstructor");
        }
    }
}
