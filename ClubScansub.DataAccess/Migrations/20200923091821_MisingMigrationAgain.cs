using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class MisingMigrationAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DeeplinkId",
                table: "Event",
                nullable: false,
                defaultValue: new Guid("2ea8aa5a-6c57-4b64-bed4-3017535c20aa"),
                oldClrType: typeof(Guid),
                oldDefaultValue: new Guid("75e0ca96-73d6-4730-8860-18c5d9b809a9"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DeeplinkId",
                table: "Event",
                nullable: false,
                defaultValue: new Guid("75e0ca96-73d6-4730-8860-18c5d9b809a9"),
                oldClrType: typeof(Guid),
                oldDefaultValue: new Guid("2ea8aa5a-6c57-4b64-bed4-3017535c20aa"));
        }
    }
}
