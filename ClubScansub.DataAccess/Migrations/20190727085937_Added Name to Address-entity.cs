using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddedNametoAddressentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Zipcode",
                table: "Addresses",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(int),
                oldMaxLength: 4);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Addresses",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Addresses");

            migrationBuilder.AlterColumn<int>(
                name: "Zipcode",
                table: "Addresses",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(int),
                oldMaxLength: 4,
                oldNullable: true);
        }
    }
}
