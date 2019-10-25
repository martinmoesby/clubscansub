using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class RemovedCourseSessionInstructorentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseSessionInstructor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseSessionInstructor",
                columns: table => new
                {
                    CourseSessionId = table.Column<int>(nullable: false),
                    InstructorId = table.Column<string>(nullable: false),
                    CourseSessionId1 = table.Column<int>(nullable: true),
                    InstructorApproved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSessionInstructor", x => new { x.CourseSessionId, x.InstructorId });
                    table.ForeignKey(
                        name: "FK_CourseSessionInstructor_CourseSessions_CourseSessionId1",
                        column: x => x.CourseSessionId1,
                        principalTable: "CourseSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseSessionInstructor_AspNetUsers_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessionInstructor_CourseSessionId1",
                table: "CourseSessionInstructor",
                column: "CourseSessionId1");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessionInstructor_InstructorId",
                table: "CourseSessionInstructor",
                column: "InstructorId");
        }
    }
}
