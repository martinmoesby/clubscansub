using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubScansub.Data.Migrations
{
    public partial class AddDeeplinkIdToEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            //migrationBuilder.AddColumn<decimal>(
            //    name: "saldo",
            //    table: "medlemsdata",
            //    nullable: false,
            //    defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "DeeplinkId",
                table: "Event",
                nullable: false,
                defaultValue: new Guid("c759e9a1-52a4-454d-ad7f-e17911816013"));

            //migrationBuilder.CreateTable(
            //    name: "kursistdata",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        fornavn = table.Column<string>(nullable: true),
            //        efternavn = table.Column<string>(nullable: true),
            //        adresse = table.Column<string>(nullable: true),
            //        post = table.Column<double>(nullable: false),
            //        telefonprivat = table.Column<string>(nullable: true),
            //        telefonBil = table.Column<string>(nullable: true),
            //        eMail = table.Column<string>(nullable: true),
            //        Certifikat = table.Column<string>(nullable: true),
            //        status = table.Column<bool>(nullable: false),
            //        cpr = table.Column<string>(nullable: true),
            //        navn = table.Column<string>(nullable: true),
            //        tlf = table.Column<string>(nullable: true),
            //        password = table.Column<string>(nullable: true),
            //        dsfnr = table.Column<string>(nullable: true),
            //        saldo = table.Column<decimal>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_kursistdata", x => x.id);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "kursistdata");

            //migrationBuilder.DropColumn(
            //    name: "saldo",
            //    table: "medlemsdata");

            migrationBuilder.DropColumn(
                name: "DeeplinkId",
                table: "Event");

            //migrationBuilder.CreateTable(
            //    name: "saldooplysning",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        dato = table.Column<DateTime>(nullable: false),
            //        dsfnr = table.Column<string>(nullable: true),
            //        password = table.Column<string>(nullable: true),
            //        pris = table.Column<decimal>(nullable: false),
            //        tekst = table.Column<string>(nullable: true),
            //        turid = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_saldooplysning", x => x.id);
            //    });
        }
    }
}
