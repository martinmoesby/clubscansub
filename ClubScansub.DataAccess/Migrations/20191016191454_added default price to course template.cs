using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class addeddefaultpricetocoursetemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DefaultPrice",
                table: "CourseTemplate",
                type: "decimal",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultPrice",
                table: "CourseTemplate");
        }
    }
}
