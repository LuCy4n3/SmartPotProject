using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SqlServerVer2.Models
{
    public partial class PotContext : DbContext
    {
        public PotContext(DbContextOptions
       <PotContext> options)
           : base(options)
        {
        }
        public virtual DbSet<Pot> Pot { get; set; }
        public virtual DbSet<Plant> Plant { get; set; }
        public virtual DbSet<User> Users { get; set; }
       
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
            modelBuilder.Entity<Pot>()
                 .HasOne<User>() // Specify the type of the navigation property
         .WithMany()      // No navigation property in the Plant class
         .HasForeignKey(p => p.UserId)
         .IsRequired();

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
