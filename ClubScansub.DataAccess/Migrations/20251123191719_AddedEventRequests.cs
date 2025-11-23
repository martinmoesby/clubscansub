using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubScansub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedEventRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dates = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DivelocationId = table.Column<int>(type: "int", nullable: true),
                    RequesterId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalParticipants = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventRequests_AspNetUsers_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventRequests_Divelocation_DivelocationId",
                        column: x => x.DivelocationId,
                        principalTable: "Divelocation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventRequests_DivelocationId",
                table: "EventRequests",
                column: "DivelocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRequests_RequesterId",
                table: "EventRequests",
                column: "RequesterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventRequests");
        }
    }
}
