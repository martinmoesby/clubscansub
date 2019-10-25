using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class changedrelationbetweenCertandDiveOrgtilam2mrelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificate_Diveorganization_DiveorganizationId",
                table: "Certificate");

            migrationBuilder.DropIndex(
                name: "IX_Certificate_DiveorganizationId",
                table: "Certificate");

            migrationBuilder.DropColumn(
                name: "DiveorganizationId",
                table: "Certificate");

            migrationBuilder.CreateTable(
                name: "DiveorgCertificates",
                columns: table => new
                {
                    DiveorganizationId = table.Column<int>(nullable: false),
                    CertificateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiveorgCertificates", x => new { x.DiveorganizationId, x.CertificateId });
                    table.UniqueConstraint("AK_DiveorgCertificates_CertificateId_DiveorganizationId", x => new { x.CertificateId, x.DiveorganizationId });
                    table.ForeignKey(
                        name: "FK_DiveorgCertificates_Certificate_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiveorgCertificates_Diveorganization_DiveorganizationId",
                        column: x => x.DiveorganizationId,
                        principalTable: "Diveorganization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiveorgCertificates");

            migrationBuilder.AddColumn<int>(
                name: "DiveorganizationId",
                table: "Certificate",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_DiveorganizationId",
                table: "Certificate",
                column: "DiveorganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificate_Diveorganization_DiveorganizationId",
                table: "Certificate",
                column: "DiveorganizationId",
                principalTable: "Diveorganization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
