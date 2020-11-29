using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class MissingMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "kursistdata");

            migrationBuilder.DropTable(
                name: "medlemsdata");

            //migrationBuilder.DropTable(
            //    name: "Site");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeeplinkId",
                table: "Event",
                nullable: false,
                defaultValue: new Guid("75e0ca96-73d6-4730-8860-18c5d9b809a9"),
                oldClrType: typeof(Guid),
                oldDefaultValue: new Guid("11c316bb-ee3b-4e86-baf3-1cfa55108cc4"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DeeplinkId",
                table: "Event",
                nullable: false,
                defaultValue: new Guid("11c316bb-ee3b-4e86-baf3-1cfa55108cc4"),
                oldClrType: typeof(Guid),
                oldDefaultValue: new Guid("75e0ca96-73d6-4730-8860-18c5d9b809a9"));

            migrationBuilder.CreateTable(
                name: "kursistdata",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Certifikat = table.Column<string>(nullable: true),
                    adresse = table.Column<string>(nullable: true),
                    cpr = table.Column<string>(nullable: true),
                    dsfnr = table.Column<string>(nullable: true),
                    eMail = table.Column<string>(nullable: true),
                    efternavn = table.Column<string>(nullable: true),
                    fornavn = table.Column<string>(nullable: true),
                    navn = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: true),
                    post = table.Column<double>(nullable: false),
                    saldo = table.Column<decimal>(nullable: false),
                    status = table.Column<bool>(nullable: false),
                    telefonBil = table.Column<string>(nullable: true),
                    telefonprivat = table.Column<string>(nullable: true),
                    tlf = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kursistdata", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "medlemsdata",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Certifikat = table.Column<string>(nullable: true),
                    adresse = table.Column<string>(nullable: true),
                    cpr = table.Column<string>(nullable: true),
                    dsfnr = table.Column<string>(nullable: true),
                    eMail = table.Column<string>(nullable: true),
                    efternavn = table.Column<string>(nullable: true),
                    fornavn = table.Column<string>(nullable: true),
                    jstatus = table.Column<bool>(nullable: false),
                    navn = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: true),
                    post = table.Column<double>(nullable: false),
                    saldo = table.Column<decimal>(nullable: false),
                    status = table.Column<bool>(nullable: false),
                    telefonBil = table.Column<string>(nullable: true),
                    telefonprivat = table.Column<string>(nullable: true),
                    tlf = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medlemsdata", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Site",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Adress = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Latitude = table.Column<decimal>(nullable: false),
                    LocationType = table.Column<int>(nullable: false),
                    Longitude = table.Column<decimal>(nullable: false),
                    MaxDivers = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Zip = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site", x => x.Id);
                });
        }
    }
}
