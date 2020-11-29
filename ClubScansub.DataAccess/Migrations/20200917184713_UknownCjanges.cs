using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class UknownCjanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DeeplinkId",
                table: "Event",
                nullable: false,
                defaultValue: Guid.NewGuid(),
                oldClrType: typeof(Guid),
                oldDefaultValue: new Guid("c759e9a1-52a4-454d-ad7f-e17911816013"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DeeplinkId",
                table: "Event",
                nullable: false,
                defaultValue: Guid.NewGuid(),
                oldClrType: typeof(Guid),
                oldDefaultValue: new Guid("11c316bb-ee3b-4e86-baf3-1cfa55108cc4"));
        }
    }
}
