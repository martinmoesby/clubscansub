using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class MadeSessionscountonCoursetemplateanautomaticcounter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcademicSessions",
                table: "CourseTemplate");

            migrationBuilder.DropColumn(
                name: "OpenWaterSessions",
                table: "CourseTemplate");

            migrationBuilder.DropColumn(
                name: "PoolSessions",
                table: "CourseTemplate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcademicSessions",
                table: "CourseTemplate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpenWaterSessions",
                table: "CourseTemplate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PoolSessions",
                table: "CourseTemplate",
                nullable: false,
                defaultValue: 0);
        }
    }
}
