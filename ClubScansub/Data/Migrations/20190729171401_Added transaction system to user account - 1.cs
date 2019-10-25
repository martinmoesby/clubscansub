using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class Addedtransactionsystemtouseraccount1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "ApplicationUserAccountEntry",
                newName: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ApplicationUserAccountEntry",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "decimal(18,2)",
                table: "ApplicationUserAccountEntry",
                newName: "Amount");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ApplicationUserAccountEntry",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);
        }
    }
}
