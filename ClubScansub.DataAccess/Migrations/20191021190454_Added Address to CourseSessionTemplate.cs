using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedAddresstoCourseSessionTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "CourseSessionTemplate",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessionTemplate_AddressId",
                table: "CourseSessionTemplate",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessionTemplate_Address_AddressId",
                table: "CourseSessionTemplate",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessionTemplate_Address_AddressId",
                table: "CourseSessionTemplate");

            migrationBuilder.DropIndex(
                name: "IX_CourseSessionTemplate_AddressId",
                table: "CourseSessionTemplate");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "CourseSessionTemplate");
        }
    }
}
