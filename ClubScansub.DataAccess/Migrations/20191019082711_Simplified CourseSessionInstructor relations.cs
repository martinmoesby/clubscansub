using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class SimplifiedCourseSessionInstructorrelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessionInstructor_CourseSessions_CourseSessionId1",
                table: "CourseSessionInstructor");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessionInstructor_CourseSessions_CourseSessionId1",
                table: "CourseSessionInstructor",
                column: "CourseSessionId1",
                principalTable: "CourseSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessionInstructor_CourseSessions_CourseSessionId1",
                table: "CourseSessionInstructor");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessionInstructor_CourseSessions_CourseSessionId1",
                table: "CourseSessionInstructor",
                column: "CourseSessionId1",
                principalTable: "CourseSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
