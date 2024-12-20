using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SQLserverFinale.Migrations
{
    /// <inheritdoc />
    public partial class PotV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plant",
                columns: table => new
                {
                    PlantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlantGroup = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WaterPref = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LifeCycle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlantHabit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FlowerColor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhMinVal = table.Column<float>(type: "real", nullable: false),
                    PhMaxVal = table.Column<float>(type: "real", nullable: false),
                    MinTemp = table.Column<int>(type: "int", nullable: false),
                    MaxTemp = table.Column<int>(type: "int", nullable: false),
                    SunReq = table.Column<int>(type: "int", nullable: false),
                    PlantHeight = table.Column<int>(type: "int", nullable: false),
                    PlantWidth = table.Column<int>(type: "int", nullable: false),
                    FruitingTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlowerTime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoilType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nitrogen = table.Column<int>(type: "int", nullable: false),
                    Phosphorus = table.Column<int>(type: "int", nullable: false),
                    Potassium = table.Column<int>(type: "int", nullable: false),
                    Spacing = table.Column<int>(type: "int", nullable: false),
                    Humidity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plant", x => x.PlantName);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPassword = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Pot",
                columns: table => new
                {
                    PotId = table.Column<int>(type: "int", nullable: false),
                    PotName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PotType = table.Column<int>(type: "int", nullable: false),
                    PlantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    HasCamera = table.Column<bool>(type: "bit", nullable: true),
                    PictReq = table.Column<bool>(type: "bit", nullable: true),
                    PumpStatus = table.Column<bool>(type: "bit", nullable: true),
                    GreenHouseStatus = table.Column<bool>(type: "bit", nullable: true),
                    GreenHouseTemperature = table.Column<double>(type: "float", nullable: true),
                    GreenHouseHumidity = table.Column<double>(type: "float", nullable: true),
                    GreenHousePressure = table.Column<double>(type: "float", nullable: true),
                    PotPotassium = table.Column<double>(type: "float", nullable: true),
                    PotPhospor = table.Column<double>(type: "float", nullable: true),
                    PotNitrogen = table.Column<double>(type: "float", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Pot_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pot_PlantName",
                table: "Pot",
                column: "PlantName");

            migrationBuilder.CreateIndex(
                name: "IX_Pot_PotName",
                table: "Pot",
                column: "PotName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pot_UserId",
                table: "Pot",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pot");

            migrationBuilder.DropTable(
                name: "Plant");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
