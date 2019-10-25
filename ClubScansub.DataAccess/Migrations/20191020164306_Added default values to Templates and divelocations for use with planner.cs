using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddeddefaultvaluestoTemplatesanddivelocationsforusewithplanner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CertificateId",
                table: "Divelocation",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DefaultDuration",
                table: "Divelocation",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DefaultStartTime",
                table: "Divelocation",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "MaxStudents",
                table: "CourseTemplate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinStudents",
                table: "CourseTemplate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Divelocation_CertificateId",
                table: "Divelocation",
                column: "CertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Divelocation_Certificate_CertificateId",
                table: "Divelocation",
                column: "CertificateId",
                principalTable: "Certificate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Divelocation_Certificate_CertificateId",
                table: "Divelocation");

            migrationBuilder.DropIndex(
                name: "IX_Divelocation_CertificateId",
                table: "Divelocation");

            migrationBuilder.DropColumn(
                name: "CertificateId",
                table: "Divelocation");

            migrationBuilder.DropColumn(
                name: "DefaultDuration",
                table: "Divelocation");

            migrationBuilder.DropColumn(
                name: "DefaultStartTime",
                table: "Divelocation");

            migrationBuilder.DropColumn(
                name: "MaxStudents",
                table: "CourseTemplate");

            migrationBuilder.DropColumn(
                name: "MinStudents",
                table: "CourseTemplate");
        }
    }
}
