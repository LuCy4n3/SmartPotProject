using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlServerVer2.Migrations.SamplePotDb
{
    /// <inheritdoc />
    public partial class initPot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.CreateTable(
                name: "Pot",
                columns: table => new
                {
                    PotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PotType = table.Column<int>(type: "int", nullable: false),
                    PlantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pot", x => x.PotId);
                    table.ForeignKey(
                        name: "FK_Pot_Plant_PlantName",
                        column: x => x.PlantName,
                        principalTable: "Plant",
                        principalColumn: "PlantName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pot_PlantName",
                table: "Pot",
                column: "PlantName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pot");

            migrationBuilder.DropTable(
                name: "Plant");
        }
    }
}
