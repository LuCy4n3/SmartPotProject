using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SqlServerVer2.Models
{
    public class Plant
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlantId { get; set; }
        [Required]
        [Key]
        [MaxLength(100)]
        public string PlantName { get; set; }
        [Required]
        [MaxLength(100)]
        public string PlantGroup { get; set; }
        [MaxLength(100)]
        public string WaterPref { get; set; }
        [MaxLength(100)]
        public string LifeCycle { get; set; }
        [MaxLength(100)]
        public string PlantHabit { get; set; }
        [MaxLength(100)]
        public string FlowerColor { get; set; }
        public int PhMinVal { get; set; }
        public int PhMaxVal { get; set; }
        public int MinTemp { get; set; }
        public int MaxTemp { get; set; }
        public int SunReq { get; set; }
        public int PlantHeight { get; set; }
        public int PlantWidth { get; set; }
        public int FruitingTime { get; set; }
        public int FlowerTime { get; set; }

    }
}
