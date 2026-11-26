using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lageros.Migrations
{
    /// <inheritdoc />
    public partial class Inicijalna : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Oprema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventarniBroj = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Vrsta = table.Column<int>(type: "int", nullable: false),
                    Proizvodac = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SerijskiBroj = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Podvrsta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DatumNabave = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Napomena = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oprema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sektori",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sektori", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zaposlenici",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Radnomjesto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Aktivan = table.Column<bool>(type: "bit", nullable: false),
                    SektorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zaposlenici", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zaposlenici_Sektori_SektorId",
                        column: x => x.SektorId,
                        principalTable: "Sektori",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zaduzenja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpremaId = table.Column<int>(type: "int", nullable: false),
                    ZaposlenikId = table.Column<int>(type: "int", nullable: false),
                    DatumZaduzenja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumRazduzenja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Napomena = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ZaduzioKorisnik = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RazduzioKorisnik = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zaduzenja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zaduzenja_Oprema_OpremaId",
                        column: x => x.OpremaId,
                        principalTable: "Oprema",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zaduzenja_Zaposlenici_ZaposlenikId",
                        column: x => x.ZaposlenikId,
                        principalTable: "Zaposlenici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Oprema_InventarniBroj",
                table: "Oprema",
                column: "InventarniBroj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zaduzenja_AktivnoPoOpremi",
                table: "Zaduzenja",
                column: "OpremaId",
                unique: true,
                filter: "[DatumRazduzenja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Zaduzenja_ZaposlenikId",
                table: "Zaduzenja",
                column: "ZaposlenikId");

            migrationBuilder.CreateIndex(
                name: "IX_Zaposlenici_SektorId",
                table: "Zaposlenici",
                column: "SektorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Zaduzenja");

            migrationBuilder.DropTable(
                name: "Oprema");

            migrationBuilder.DropTable(
                name: "Zaposlenici");

            migrationBuilder.DropTable(
                name: "Sektori");
        }
    }
}
