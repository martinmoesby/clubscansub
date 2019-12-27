using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class addedolddatabaseinformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "medlemsdata",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    fornavn = table.Column<string>(nullable: true),
                    efternavn = table.Column<string>(nullable: true),
                    adresse = table.Column<string>(nullable: true),
                    post = table.Column<double>(nullable: false),
                    telefonprivat = table.Column<string>(nullable: true),
                    telefonBil = table.Column<string>(nullable: true),
                    eMail = table.Column<string>(nullable: true),
                    Certifikat = table.Column<string>(nullable: true),
                    status = table.Column<bool>(nullable: false),
                    jstatus = table.Column<bool>(nullable: false),
                    cpr = table.Column<string>(nullable: true),
                    navn = table.Column<string>(nullable: true),
                    tlf = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: true),
                    dsfnr = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medlemsdata", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "saldooplysning",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    password = table.Column<string>(nullable: true),
                    dato = table.Column<DateTime>(nullable: false),
                    tekst = table.Column<string>(nullable: true),
                    turid = table.Column<int>(nullable: false),
                    pris = table.Column<decimal>(nullable: false),
                    dsfnr = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saldooplysning", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "medlemsdata");

            migrationBuilder.DropTable(
                name: "saldooplysning");
        }
    }
}
