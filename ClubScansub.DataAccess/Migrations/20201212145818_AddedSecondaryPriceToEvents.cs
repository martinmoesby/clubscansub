using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedSecondaryPriceToEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PremiumPrice",
                table: "Event",
                type: "decimal",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PremiumPrice",
                table: "Event");
        }
    }
}
