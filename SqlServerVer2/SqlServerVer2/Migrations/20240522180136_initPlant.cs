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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plant");
        }
    }
}
