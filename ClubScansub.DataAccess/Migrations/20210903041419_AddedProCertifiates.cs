using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedProCertifiates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstructorCertificateId",
                table: "CourseTemplate",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiveproCertificate",
                table: "Certificate",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CourseTemplate_InstructorCertificateId",
                table: "CourseTemplate",
                column: "InstructorCertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTemplate_Certificate_InstructorCertificateId",
                table: "CourseTemplate",
                column: "InstructorCertificateId",
                principalTable: "Certificate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseTemplate_Certificate_InstructorCertificateId",
                table: "CourseTemplate");

            migrationBuilder.DropIndex(
                name: "IX_CourseTemplate_InstructorCertificateId",
                table: "CourseTemplate");

            migrationBuilder.DropColumn(
                name: "InstructorCertificateId",
                table: "CourseTemplate");

            migrationBuilder.DropColumn(
                name: "IsDiveproCertificate",
                table: "Certificate");
        }
    }
}
