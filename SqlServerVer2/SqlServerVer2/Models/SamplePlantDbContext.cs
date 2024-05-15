using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SqlServerVer2.Models
{
    public partial class SamplePlantDbContext : DbContext
    {
        public SamplePlantDbContext(DbContextOptions
       <SamplePlantDbContext> options)
           : base(options)
        {
        }
        public virtual DbSet<Plant> Plant { get; set; }
        public async Task<List<Plant>> SearchRecords(string searchString)
        {
            return await Plant.FromSqlRaw("EXEC SearchPlantByName " + searchString+"",

                                            new SqlParameter("@SearchString", searchString))
                               .ToListAsync();
          
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Plant>(entity => {
                entity.HasKey(k => k.PlantName);
            });
            
            /*  modelBuilder.Entity<Plant>()
                 .HasIndex(p => p.PlantName)
                 .IsUnique();
              OnModelCreatingPartial(modelBuilder);*/

        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
