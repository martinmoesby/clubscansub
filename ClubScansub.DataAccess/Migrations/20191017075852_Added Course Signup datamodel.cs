using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedCourseSignupdatamodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessions_Event_CourseId",
                table: "CourseSessions");

            migrationBuilder.CreateTable(
                name: "CourseSignup",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(nullable: false),
                    CourseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSignup", x => new { x.ApplicationUserId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_CourseSignup_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseSignup_Event_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseSignup_CourseId",
                table: "CourseSignup",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessions_Event_CourseId",
                table: "CourseSessions",
                column: "CourseId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessions_Event_CourseId",
                table: "CourseSessions");

            migrationBuilder.DropTable(
                name: "CourseSignup");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessions_Event_CourseId",
                table: "CourseSessions",
                column: "CourseId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
