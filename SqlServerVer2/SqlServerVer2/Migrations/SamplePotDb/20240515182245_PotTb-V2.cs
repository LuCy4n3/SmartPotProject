using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlServerVer2.Migrations.SamplePotDb
{
    /// <inheritdoc />
    public partial class PotTbV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pot_PlantName",
                table: "Pot");

           

            migrationBuilder.CreateIndex(
                name: "IX_Pot_PlantName",
                table: "Pot",
                column: "PlantName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pot_PlantName",
                table: "Pot");

            

            migrationBuilder.CreateIndex(
                name: "IX_Pot_PlantName",
                table: "Pot",
                column: "PlantName",
                unique: true);
        }
    }
}
