using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SUUO_DZ3.Migrations
{
    /// <inheritdoc />
    public partial class MaknutiDuplikati : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Narudzbe_Konobari_KonobarId",
                table: "Narudzbe");

            migrationBuilder.DropForeignKey(
                name: "FK_StavkeNarudzbe_Kuhari_KuharIdKuhar",
                table: "StavkeNarudzbe");

            migrationBuilder.DropIndex(
                name: "IX_StavkeNarudzbe_KuharIdKuhar",
                table: "StavkeNarudzbe");

            migrationBuilder.DropColumn(
                name: "KuharIdKuhar",
                table: "StavkeNarudzbe");

            migrationBuilder.AddForeignKey(
                name: "FK_Narudzbe_Konobari_KonobarId",
                table: "Narudzbe",
                column: "KonobarId",
                principalTable: "Konobari",
                principalColumn: "IdKonobar",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Narudzbe_Konobari_KonobarId",
                table: "Narudzbe");

            migrationBuilder.AddColumn<Guid>(
                name: "KuharIdKuhar",
                table: "StavkeNarudzbe",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StavkeNarudzbe_KuharIdKuhar",
                table: "StavkeNarudzbe",
                column: "KuharIdKuhar");

            migrationBuilder.AddForeignKey(
                name: "FK_Narudzbe_Konobari_KonobarId",
                table: "Narudzbe",
                column: "KonobarId",
                principalTable: "Konobari",
                principalColumn: "IdKonobar",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StavkeNarudzbe_Kuhari_KuharIdKuhar",
                table: "StavkeNarudzbe",
                column: "KuharIdKuhar",
                principalTable: "Kuhari",
                principalColumn: "IdKuhar");
        }
    }
}
