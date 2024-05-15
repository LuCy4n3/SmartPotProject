using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;

namespace SqlServerVer2.Models
{
    public class Pot
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PotId { get; set; }
        [Required]
        public int PotType { get; set; }
        [ForeignKey("Plant")]
        [MaxLength(100)]
        public string PlantName { get; set; }

        internal static object FromSqlRaw(string v, SqlParameter sqlParameter)
        {
            throw new NotImplementedException();
        }
        // public Plant Plant { get; set; }

    }
}
