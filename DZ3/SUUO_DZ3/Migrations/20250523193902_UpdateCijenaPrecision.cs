using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SUUO_DZ3.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCijenaPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MetodaPlacanja",
                table: "Narudzbe",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MetodaPlacanja",
                table: "Narudzbe",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
