using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace SQLserverFinale.Models
{
    [Index(nameof(Pot.PotName), IsUnique = true)] 
    public class Pot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PotId { get; set; }
        [Required,NotNull]
        public string PotName { get; set; }
        [Required]
        public int PotType { get; set; }
        [ForeignKey("Plant")]
        [MaxLength(100)]
        public string PlantName { get; set; }

        internal static object FromSqlRaw(string v, SqlParameter sqlParameter)
        {
            throw new NotImplementedException();
        }
        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }
        public bool? HasCamera { get; set; }
        public bool? PictReq { get; set; }
        public bool? PumpStatus { get; set; }
        public bool? GreenHouseStatus { get; set; }
        public double? GreenHouseTemperature { get; set; }
        public double? GreenHouseHumidity { get; set; }
        public double? GreenHousePressure { get; set; }
        public double? PotPotassium {  get; set; }
        public double? PotPhospor {  get; set; }
        public double? PotNitrogen { get; set; }
        // public Plant Plant { get; set; }

    }
}
