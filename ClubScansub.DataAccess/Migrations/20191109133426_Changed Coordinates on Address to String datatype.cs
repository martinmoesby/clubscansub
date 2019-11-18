using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class ChangedCoordinatesonAddresstoStringdatatype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Longitude",
                table: "Address",
                nullable: true,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<string>(
                name: "Latitude",
                table: "Address",
                nullable: true,
                oldClrType: typeof(float));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Longitude",
                table: "Address",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Latitude",
                table: "Address",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
