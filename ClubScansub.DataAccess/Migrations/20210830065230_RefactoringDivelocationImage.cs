using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class RefactoringDivelocationImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Divelocation");

            migrationBuilder.CreateTable(
                name: "DivelocationImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DivelocationId = table.Column<int>(nullable: false),
                    ImageData = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DivelocationImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DivelocationImages_Divelocation_DivelocationId",
                        column: x => x.DivelocationId,
                        principalTable: "Divelocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DivelocationImages_DivelocationId",
                table: "DivelocationImages",
                column: "DivelocationId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DivelocationImages");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Divelocation",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
