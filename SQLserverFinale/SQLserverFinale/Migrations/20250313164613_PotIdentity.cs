using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SQLserverFinale.Migrations
{
    /// <inheritdoc />
    public partial class PotIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop the existing primary key constraint (if any)
            migrationBuilder.DropPrimaryKey(
                name: "PK_Pot",
                table: "Pot");

            // Step 2: Drop the existing PotId column
            migrationBuilder.DropColumn(
                name: "PotId",
                table: "Pot");

            // Step 3: Recreate the PotId column with the IDENTITY property
            migrationBuilder.AddColumn<int>(
                name: "PotId",
                table: "Pot",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            // Step 4: Re-add the primary key constraint
            migrationBuilder.AddPrimaryKey(
                name: "PK_Pot",
                table: "Pot",
                column: "PotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop the primary key constraint
            migrationBuilder.DropPrimaryKey(
                name: "PK_Pot",
                table: "Pot");

            // Step 2: Drop the PotId column with the IDENTITY property
            migrationBuilder.DropColumn(
                name: "PotId",
                table: "Pot");

            // Step 3: Recreate the PotId column without the IDENTITY property
            migrationBuilder.AddColumn<int>(
                name: "PotId",
                table: "Pot",
                type: "int",
                nullable: false);

            // Step 4: Re-add the primary key constraint
            migrationBuilder.AddPrimaryKey(
                name: "PK_Pot",
                table: "Pot",
                column: "PotId");
        }
    }
}