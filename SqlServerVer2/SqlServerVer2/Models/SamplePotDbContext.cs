using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SqlServerVer2.Models
{
    public partial class SamplePotDbContext : DbContext
    {
        public SamplePotDbContext(DbContextOptions
       <SamplePotDbContext> options)
           : base(options)
        {
        }
        public virtual DbSet<Pot> Pot { get; set; }
        public virtual DbSet<Plant> Plant { get; set; }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pot>(entity => {
                entity.HasKey(k => k.PotId);
            });

            modelBuilder.Entity<Pot>()
         .HasOne<Plant>() // Specify the type of the navigation property
         .WithMany()      // No navigation property in the Plant class
         .HasForeignKey(p => p.PlantName)
         .IsRequired();
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
