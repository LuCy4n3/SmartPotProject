using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;


namespace SQLserverFinale.Models
{
    public partial class UserContext : DbContext
    {
        public UserContext(DbContextOptions
      <UserContext> options)
          : base(options)
        {
        }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => {
                entity.HasKey(k => k.UserId);
            });

            /*  modelBuilder.Entity<Plant>()
                 .HasIndex(p => p.PlantName)
                 .IsUnique();
              OnModelCreatingPartial(modelBuilder);*/

        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);         
    }
}
