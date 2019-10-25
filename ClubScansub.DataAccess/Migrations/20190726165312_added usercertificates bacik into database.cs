using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class addedusercertificatesbacikintodatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserCertificate",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCertificate_UserId",
                table: "UserCertificate",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCertificate_AspNetUsers_UserId",
                table: "UserCertificate",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCertificate_AspNetUsers_UserId",
                table: "UserCertificate");

            migrationBuilder.DropIndex(
                name: "IX_UserCertificate_UserId",
                table: "UserCertificate");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserCertificate");
        }
    }
}
