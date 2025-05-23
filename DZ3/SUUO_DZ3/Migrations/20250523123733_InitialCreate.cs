using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SUUO_DZ3.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Konobari",
                columns: table => new
                {
                    IdKonobar = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktivan = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Konobari", x => x.IdKonobar);
                });

            migrationBuilder.CreateTable(
                name: "Kuhari",
                columns: table => new
                {
                    IdKuhar = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktivan = table.Column<bool>(type: "bit", nullable: false),
                    Specijaliteti = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kuhari", x => x.IdKuhar);
                });

            migrationBuilder.CreateTable(
                name: "Narudzbe",
                columns: table => new
                {
                    NarudzbaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VrijemeNarudzbe = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KonobarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KuharId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Stol = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Narudzbe", x => x.NarudzbaId);
                    table.ForeignKey(
                        name: "FK_Narudzbe_Konobari_KonobarId",
                        column: x => x.KonobarId,
                        principalTable: "Konobari",
                        principalColumn: "IdKonobar",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Narudzbe_Kuhari_KuharId",
                        column: x => x.KuharId,
                        principalTable: "Kuhari",
                        principalColumn: "IdKuhar",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StavkeNarudzbe",
                columns: table => new
                {
                    StavkaNarudzbeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NarudzbaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kolicina = table.Column<int>(type: "int", nullable: false),
                    Cijena = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AkcijskaPonuda = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    KuharIdKuhar = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StavkeNarudzbe", x => x.StavkaNarudzbeId);
                    table.ForeignKey(
                        name: "FK_StavkeNarudzbe_Kuhari_KuharIdKuhar",
                        column: x => x.KuharIdKuhar,
                        principalTable: "Kuhari",
                        principalColumn: "IdKuhar");
                    table.ForeignKey(
                        name: "FK_StavkeNarudzbe_Narudzbe_NarudzbaId",
                        column: x => x.NarudzbaId,
                        principalTable: "Narudzbe",
                        principalColumn: "NarudzbaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Narudzbe_KonobarId",
                table: "Narudzbe",
                column: "KonobarId");

            migrationBuilder.CreateIndex(
                name: "IX_Narudzbe_KuharId",
                table: "Narudzbe",
                column: "KuharId");

            migrationBuilder.CreateIndex(
                name: "IX_StavkeNarudzbe_KuharIdKuhar",
                table: "StavkeNarudzbe",
                column: "KuharIdKuhar");

            migrationBuilder.CreateIndex(
                name: "IX_StavkeNarudzbe_NarudzbaId",
                table: "StavkeNarudzbe",
                column: "NarudzbaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StavkeNarudzbe");

            migrationBuilder.DropTable(
                name: "Narudzbe");

            migrationBuilder.DropTable(
                name: "Konobari");

            migrationBuilder.DropTable(
                name: "Kuhari");
        }
    }
}
