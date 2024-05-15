using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;

#nullable disable

namespace SqlServerVer2.Migrations
{
    /// <inheritdoc />
    public partial class StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            StringBuilder storedProcedureCode = new StringBuilder();

            storedProcedureCode.Append("CREATE PROCEDURE [dbo].[SearchPlantByName]" + Environment.NewLine);
            storedProcedureCode.Append(" @SearchString NVARCHAR(100)" + Environment.NewLine);
            storedProcedureCode.Append("AS" + Environment.NewLine);
            storedProcedureCode.Append("BEGIN" + Environment.NewLine);
            storedProcedureCode.Append("SELECT * FROM Plant" + Environment.NewLine);
            storedProcedureCode.Append("WHERE PlantName LIKE '%' + @SearchString + '%'" + Environment.NewLine);
            storedProcedureCode.Append("END" + Environment.NewLine);

           migrationBuilder.Sql(storedProcedureCode.ToString());
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[SearchPlantByName] ");
        }
    }
}
