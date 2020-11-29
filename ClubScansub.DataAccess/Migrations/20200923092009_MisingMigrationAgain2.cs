using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class MisingMigrationAgain2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DeeplinkId",
                table: "Event",
                nullable: false,
                defaultValue: new Guid("e38fe057-d692-493c-8093-94a6c4bfcc20"),
                oldClrType: typeof(Guid),
                oldDefaultValue: new Guid("2ea8aa5a-6c57-4b64-bed4-3017535c20aa"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DeeplinkId",
                table: "Event",
                nullable: false,
                defaultValue: new Guid("2ea8aa5a-6c57-4b64-bed4-3017535c20aa"),
                oldClrType: typeof(Guid),
                oldDefaultValue: new Guid("e38fe057-d692-493c-8093-94a6c4bfcc20"));
        }
    }
}
