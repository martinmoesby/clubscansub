using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class MisingMigrationAgain3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DeeplinkId",
                table: "Event",
                nullable: false,
                oldClrType: typeof(Guid),
                oldDefaultValue: new Guid("e38fe057-d692-493c-8093-94a6c4bfcc20"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DeeplinkId",
                table: "Event",
                nullable: false,
                defaultValue: new Guid("e38fe057-d692-493c-8093-94a6c4bfcc20"),
                oldClrType: typeof(Guid));
        }
    }
}
