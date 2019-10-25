using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedInstructorstoCourseSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstructorId",
                table: "Event",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstructorId",
                table: "Certificate",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseSessionInstructor",
                columns: table => new
                {
                    CourseSessionId = table.Column<int>(nullable: false),
                    InstructorId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSessionInstructor", x => new { x.CourseSessionId, x.InstructorId });
                    table.ForeignKey(
                        name: "FK_CourseSessionInstructor_CourseSessions_CourseSessionId",
                        column: x => x.CourseSessionId,
                        principalTable: "CourseSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseSessionInstructor_AspNetUsers_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_InstructorId",
                table: "Event",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_InstructorId",
                table: "Certificate",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessionInstructor_InstructorId",
                table: "CourseSessionInstructor",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificate_AspNetUsers_InstructorId",
                table: "Certificate",
                column: "InstructorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_AspNetUsers_InstructorId",
                table: "Event",
                column: "InstructorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificate_AspNetUsers_InstructorId",
                table: "Certificate");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_AspNetUsers_InstructorId",
                table: "Event");

            migrationBuilder.DropTable(
                name: "CourseSessionInstructor");

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
        }
    }
}
