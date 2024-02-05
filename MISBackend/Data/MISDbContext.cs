using Microsoft.EntityFrameworkCore;
using MISBackend.Data.Entity;

namespace MISBackend.Data
{
    public class MISDbContext : DbContext
    {
        public MISDbContext(DbContextOptions<MISDbContext> options)
        : base(options)
        {
        }

        // Entities
        public DbSet<DoctorModel> Doctors { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Doctor
            modelBuilder.Entity<DoctorModel>()
                .HasIndex(p => p.Email)
                .IsUnique();
            modelBuilder.Entity<DoctorModel>()
                .Property(p => p.Email)
                .HasColumnType("nvarchar(100)");
            modelBuilder.Entity<DoctorModel>()
                .Property(p => p.Name)
                .HasColumnType("nvarchar(255)");
            modelBuilder.Entity<DoctorModel>()
                .Property(p => p.Password)
                .HasColumnType("nvarchar(255)");
            modelBuilder.Entity<DoctorModel>()
                .Property(p => p.Gender)
                .HasColumnType("nvarchar(50)");
            modelBuilder.Entity<DoctorModel>()
                .Property(p => p.Gender)
                .HasConversion<string>()
                .HasColumnType("nvarchar(50)");
        }
    }
}
