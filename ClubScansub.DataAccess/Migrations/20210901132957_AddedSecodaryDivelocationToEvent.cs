using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedSecodaryDivelocationToEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SecondaryDivelocationId",
                table: "Event",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_SecondaryDivelocationId",
                table: "Event",
                column: "SecondaryDivelocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Divelocation_SecondaryDivelocationId",
                table: "Event",
                column: "SecondaryDivelocationId",
                principalTable: "Divelocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Divelocation_SecondaryDivelocationId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_SecondaryDivelocationId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "SecondaryDivelocationId",
                table: "Event");
        }
    }
}
