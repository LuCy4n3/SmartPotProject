using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SQLserverFinale.Migrations
{
    /// <inheritdoc />
    public partial class PlantV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FlowerTime",
                table: "Plant",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
            migrationBuilder.AlterColumn<string>(
                name: "FruitingTime",
                table: "Plant",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
