using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SUUO_DZ3.Migrations
{
    /// <inheritdoc />
    public partial class AddKuharUStavkuNar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "KuharId",
                table: "StavkeNarudzbe",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StavkeNarudzbe_KuharId",
                table: "StavkeNarudzbe",
                column: "KuharId");

            migrationBuilder.AddForeignKey(
                name: "FK_StavkeNarudzbe_Kuhari_KuharId",
                table: "StavkeNarudzbe",
                column: "KuharId",
                principalTable: "Kuhari",
                principalColumn: "IdKuhar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StavkeNarudzbe_Kuhari_KuharId",
                table: "StavkeNarudzbe");

            migrationBuilder.DropIndex(
                name: "IX_StavkeNarudzbe_KuharId",
                table: "StavkeNarudzbe");

            migrationBuilder.DropColumn(
                name: "KuharId",
                table: "StavkeNarudzbe");
        }
    }
}
