using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedCascadingdeleteonmanytomanyrelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCertificate_Certificate_CertificateId",
                table: "UserCertificate");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCertificate_AspNetUsers_UserId",
                table: "UserCertificate");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCertificate_Certificate_CertificateId",
                table: "UserCertificate",
                column: "CertificateId",
                principalTable: "Certificate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCertificate_AspNetUsers_UserId",
                table: "UserCertificate",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCertificate_Certificate_CertificateId",
                table: "UserCertificate");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCertificate_AspNetUsers_UserId",
                table: "UserCertificate");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCertificate_Certificate_CertificateId",
                table: "UserCertificate",
                column: "CertificateId",
                principalTable: "Certificate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCertificate_AspNetUsers_UserId",
                table: "UserCertificate",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
