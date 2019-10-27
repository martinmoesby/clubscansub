using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedClubinformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Divelocation_DivelocationId",
                table: "Event");

            migrationBuilder.CreateTable(
                name: "Club",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Tagline = table.Column<string>(nullable: true),
                    Logo = table.Column<byte[]>(nullable: true),
                    TermsText = table.Column<string>(nullable: true),
                    PrivacyText = table.Column<string>(nullable: true),
                    AddressId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Club", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Club_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Club_AddressId",
                table: "Club",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Divelocation_DivelocationId",
                table: "Event",
                column: "DivelocationId",
                principalTable: "Divelocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Divelocation_DivelocationId",
                table: "Event");

            migrationBuilder.DropTable(
                name: "Club");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Divelocation_DivelocationId",
                table: "Event",
                column: "DivelocationId",
                principalTable: "Divelocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
