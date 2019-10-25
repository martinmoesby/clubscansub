using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedCoursesToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseName",
                table: "Event",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseTemplateId",
                table: "Event",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseType",
                table: "Event",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Event",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CourseSessions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateTime = table.Column<string>(nullable: true),
                    Duration = table.Column<string>(nullable: true),
                    Sessiontype = table.Column<int>(nullable: false),
                    CourseId = table.Column<int>(nullable: true),
                    AddressId = table.Column<int>(nullable: true),
                    DivelocationId = table.Column<int>(nullable: true),
                    CourseSessionTemplateId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseSessions_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseSessions_Event_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseSessions_CourseSessionTemplate_CourseSessionTemplateId",
                        column: x => x.CourseSessionTemplateId,
                        principalTable: "CourseSessionTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseSessions_Divelocation_DivelocationId",
                        column: x => x.DivelocationId,
                        principalTable: "Divelocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_CourseTemplateId",
                table: "Event",
                column: "CourseTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessions_AddressId",
                table: "CourseSessions",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessions_CourseId",
                table: "CourseSessions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessions_CourseSessionTemplateId",
                table: "CourseSessions",
                column: "CourseSessionTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessions_DivelocationId",
                table: "CourseSessions",
                column: "DivelocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_CourseTemplate_CourseTemplateId",
                table: "Event",
                column: "CourseTemplateId",
                principalTable: "CourseTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_CourseTemplate_CourseTemplateId",
                table: "Event");

            migrationBuilder.DropTable(
                name: "CourseSessions");

            migrationBuilder.DropIndex(
                name: "IX_Event_CourseTemplateId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "CourseName",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "CourseTemplateId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "CourseType",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Event");
        }
    }
}
