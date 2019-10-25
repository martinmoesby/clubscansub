using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedCertifikateverificationtoUserCertificateentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "UserCertificate",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerifiedById",
                table: "UserCertificate",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedDate",
                table: "UserCertificate",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_UserCertificate_VerifiedById",
                table: "UserCertificate",
                column: "VerifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCertificate_AspNetUsers_VerifiedById",
                table: "UserCertificate",
                column: "VerifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCertificate_AspNetUsers_VerifiedById",
                table: "UserCertificate");

            migrationBuilder.DropIndex(
                name: "IX_UserCertificate_VerifiedById",
                table: "UserCertificate");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "UserCertificate");

            migrationBuilder.DropColumn(
                name: "VerifiedById",
                table: "UserCertificate");

            migrationBuilder.DropColumn(
                name: "VerifiedDate",
                table: "UserCertificate");
        }
    }
}
