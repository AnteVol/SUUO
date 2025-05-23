using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SUUO_DZ3.Migrations
{
    /// <inheritdoc />
    public partial class RemoveKuharFromNarudzba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Narudzbe_Kuhari_KuharId",
                table: "Narudzbe");

            migrationBuilder.DropIndex(
                name: "IX_Narudzbe_KuharId",
                table: "Narudzbe");

            migrationBuilder.DropColumn(
                name: "KuharId",
                table: "Narudzbe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "KuharId",
                table: "Narudzbe",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Narudzbe_KuharId",
                table: "Narudzbe",
                column: "KuharId");

            migrationBuilder.AddForeignKey(
                name: "FK_Narudzbe_Kuhari_KuharId",
                table: "Narudzbe",
                column: "KuharId",
                principalTable: "Kuhari",
                principalColumn: "IdKuhar",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
