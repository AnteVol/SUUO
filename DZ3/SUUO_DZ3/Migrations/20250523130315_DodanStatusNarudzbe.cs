using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SUUO_DZ3.Migrations
{
    /// <inheritdoc />
    public partial class DodanStatusNarudzbe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Narudzbe",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Narudzbe");
        }
    }
}
