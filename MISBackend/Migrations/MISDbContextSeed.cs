using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MISBackend.Data;

namespace MISBackend.Migrations
{
    public class MISDbContextSeed
    {
        private readonly MISDbContext _context;
        public MISDbContextSeed(MISDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Logika seeding lainnya
            await SeedSuperAdminAsync();

            await _context.SaveChangesAsync(); // Misalnya, menyimpan perubahan ke database
        }

        private readonly Guid IdDoctor = Guid.Parse("9713F92C-2D39-4857-95BF-E6EEC730625C");
        private readonly Guid IdSpeciality = Guid.Parse("79AFC143-C02F-4817-A9C5-E16CEE07CE96");

        public async Task SeedSuperAdminAsync()
        {
            //Seed Default User
            var defaultUser = new Data.Entity.Doctor()
            {
                Id = IdDoctor,
                Email = "superadmin@gmail.com",
                Name = "Super Doctor",
                Password = "ABC12345",
                Phone = "0878978989",
                Birthday = DateTime.Parse("1989-01-01"),
                CreateTime = DateTime.Now,
                Gender = Enum.DataEnum.Gender.Male,
                Speciality = IdSpeciality
            };
            if (await _context.Doctors.FirstOrDefaultAsync(o => o.Id == defaultUser.Id) == null)
            {
                await _context.Doctors.AddAsync(defaultUser);
            }
        }
    }
}
