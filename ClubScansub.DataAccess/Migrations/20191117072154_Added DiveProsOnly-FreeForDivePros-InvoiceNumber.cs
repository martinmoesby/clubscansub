using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedDiveProsOnlyFreeForDiveProsInvoiceNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFreeForDivepros",
                table: "Event",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInternal",
                table: "Event",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "ApplicationUserAccountEntry",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFreeForDivepros",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "IsInternal",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "ApplicationUserAccountEntry");
        }
    }
}
