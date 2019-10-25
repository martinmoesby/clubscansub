using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class Addedrelationbetweeneventandcertificate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequiredCertificateId",
                table: "Event",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_RequiredCertificateId",
                table: "Event",
                column: "RequiredCertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Certificate_RequiredCertificateId",
                table: "Event",
                column: "RequiredCertificateId",
                principalTable: "Certificate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Certificate_RequiredCertificateId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_RequiredCertificateId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "RequiredCertificateId",
                table: "Event");
        }
    }
}
