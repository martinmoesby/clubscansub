using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedCoursetemplatestoDababase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Site");

            migrationBuilder.CreateTable(
                name: "CourseTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TemplateName = table.Column<string>(nullable: true),
                    TemplateDurationInWeeks = table.Column<string>(nullable: true),
                    AcademicSessions = table.Column<int>(nullable: false),
                    PoolSessions = table.Column<int>(nullable: false),
                    OpenWaterSessions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseSessionTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SessionNumber = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    SessionType = table.Column<int>(nullable: false),
                    DefaultStartTime = table.Column<TimeSpan>(nullable: false),
                    DefaultDuration = table.Column<TimeSpan>(nullable: false),
                    DefaultWeekday = table.Column<int>(nullable: false),
                    CourseTemplateId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSessionTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseSessionTemplate_CourseTemplate_CourseTemplateId",
                        column: x => x.CourseTemplateId,
                        principalTable: "CourseTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessionTemplate_CourseTemplateId",
                table: "CourseSessionTemplate",
                column: "CourseTemplateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseSessionTemplate");

            migrationBuilder.DropTable(
                name: "CourseTemplate");

            migrationBuilder.CreateTable(
                name: "Site",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Adress = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Latitude = table.Column<float>(nullable: false),
                    Longitude = table.Column<float>(nullable: false),
                    MaxDivers = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Zip = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site", x => x.Id);
                });
        }
    }
}
