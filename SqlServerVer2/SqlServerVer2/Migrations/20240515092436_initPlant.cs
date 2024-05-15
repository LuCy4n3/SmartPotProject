using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlServerVer2.Migrations
{
    /// <inheritdoc />
    public partial class initPlant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Plant_PlantName",
                table: "Plant");

            migrationBuilder.AlterColumn<string>(
                name: "PlantName",
                table: "Plant",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Plant_PlantName",
                table: "Plant",
                column: "PlantName",
                unique: true,
                filter: "[PlantName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Plant_PlantName",
                table: "Plant");

            migrationBuilder.AlterColumn<string>(
                name: "PlantName",
                table: "Plant",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plant_PlantName",
                table: "Plant",
                column: "PlantName",
                unique: true);
        }
    }
}
