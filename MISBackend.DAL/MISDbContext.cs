using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MISBackend.DAL.Entity;

namespace MISBackend.DAL
{
    public class MISDbContext : IdentityDbContext<IdentityUser>
    {
        public MISDbContext(DbContextOptions<MISDbContext> options)
        : base(options)
        {
        }

        // Entities
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Doctor
            modelBuilder.Entity<Doctor>()
                .HasIndex(p => p.Email)
                .IsUnique();
            modelBuilder.Entity<Doctor>()
                .Property(p => p.Email)
                .HasColumnType("nvarchar(100)");
            modelBuilder.Entity<Doctor>()
                .Property(p => p.Name)
                .HasColumnType("nvarchar(255)");
            modelBuilder.Entity<Doctor>()
                .Property(p => p.Password)
                .HasColumnType("nvarchar(255)");
            modelBuilder.Entity<Doctor>()
                .Property(p => p.Gender)
                .HasColumnType("nvarchar(50)");
            modelBuilder.Entity<Doctor>()
                .Property(p => p.Gender)
                .HasConversion<string>()
                .HasColumnType("nvarchar(50)");

            //Patient
            modelBuilder.Entity<Patient>()
                .Property(p => p.Name)
                .HasColumnType("nvarchar(255)");
            modelBuilder.Entity<Patient>()
                .Property(p => p.Gender)
                .HasConversion<string>()
                .HasColumnType("nvarchar(50)");
        }
    }
}
