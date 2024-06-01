using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SQLserverFinale.Migrations.Pot
{
    /// <inheritdoc />
    public partial class initPot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plant",
                columns: table => new
                {
                    PlantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantGroup = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WaterPref = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LifeCycle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlantHabit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FlowerColor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhMinVal = table.Column<int>(type: "int", nullable: false),
                    PhMaxVal = table.Column<int>(type: "int", nullable: false),
                    MinTemp = table.Column<int>(type: "int", nullable: false),
                    MaxTemp = table.Column<int>(type: "int", nullable: false),
                    SunReq = table.Column<int>(type: "int", nullable: false),
                    PlantHeight = table.Column<int>(type: "int", nullable: false),
                    PlantWidth = table.Column<int>(type: "int", nullable: false),
                    FruitingTime = table.Column<int>(type: "int", nullable: false),
                    FlowerTime = table.Column<int>(type: "int", nullable: false)
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
                    PumpStatus = table.Column<bool>(type: "bit", nullable: false),
                    GreenHouseStatus = table.Column<bool>(type: "bit", nullable: false),
                    GreenHouseTemperature = table.Column<double>(type: "float", nullable: false),
                    GreenHouseHumidity = table.Column<double>(type: "float", nullable: false),
                    GreenHousePressure = table.Column<double>(type: "float", nullable: false),
                    PotPotassium = table.Column<double>(type: "float", nullable: false),
                    PotPhospor = table.Column<double>(type: "float", nullable: false),
                    PotNitrogen = table.Column<double>(type: "float", nullable: false)
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
