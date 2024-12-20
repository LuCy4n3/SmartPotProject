using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SQLserverFinale.Models
{
    public class Plant
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
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

        public float PhMinVal { get; set; }
        public float PhMaxVal { get; set; }
        public int MinTemp { get; set; }
        public int MaxTemp { get; set; }
        public int SunReq { get; set; }
        public int PlantHeight { get; set; }
        public int PlantWidth { get; set; }

        public string FruitingTime { get; set; }

        [MaxLength(100)]
        public string FlowerTime { get; set; }

        [MaxLength(100)]
        public string SoilType { get; set; }

        // Remove [MaxLength] here, as it's invalid on numeric types
        public int Nitrogen { get; set; }

        public int Phosphorus { get; set; }
        public int Potassium { get; set; }
        public int Spacing { get; set; }
        public int Humidity { get; set; }
    }
}
