using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class RemovedInstructorentityfromDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificate_AspNetUsers_InstructorId",
                table: "Certificate");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessionInstructor_CourseSessions_CourseSessionId",
                table: "CourseSessionInstructor");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_AspNetUsers_InstructorId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_InstructorId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Certificate_InstructorId",
                table: "Certificate");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "Certificate");

            migrationBuilder.AddColumn<int>(
                name: "CourseSessionId1",
                table: "CourseSessionInstructor",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessionInstructor_CourseSessionId1",
                table: "CourseSessionInstructor",
                column: "CourseSessionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessionInstructor_CourseSessions_CourseSessionId1",
                table: "CourseSessionInstructor",
                column: "CourseSessionId1",
                principalTable: "CourseSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessionInstructor_CourseSessions_CourseSessionId1",
                table: "CourseSessionInstructor");

            migrationBuilder.DropIndex(
                name: "IX_CourseSessionInstructor_CourseSessionId1",
                table: "CourseSessionInstructor");

            migrationBuilder.DropColumn(
                name: "CourseSessionId1",
                table: "CourseSessionInstructor");

            migrationBuilder.AddColumn<string>(
                name: "InstructorId",
                table: "Event",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstructorId",
                table: "Certificate",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_InstructorId",
                table: "Event",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_InstructorId",
                table: "Certificate",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificate_AspNetUsers_InstructorId",
                table: "Certificate",
                column: "InstructorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessionInstructor_CourseSessions_CourseSessionId",
                table: "CourseSessionInstructor",
                column: "CourseSessionId",
                principalTable: "CourseSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_AspNetUsers_InstructorId",
                table: "Event",
                column: "InstructorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
