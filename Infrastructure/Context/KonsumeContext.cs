using DaticianProj.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Project.Models.Entities;

namespace DaticianProj.Infrastructure.Context
{
    public class KonsumeContext : DbContext
    {
        public KonsumeContext(DbContextOptions<KonsumeContext> opt) : base(opt)
        {
            
        }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Profile> Profiles => Set<Profile>();
        public DbSet<UserInteraction> UserInteractions => Set<UserInteraction>();
        public DbSet<VerificationCode> VerificationCodes => Set<VerificationCode>();


        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().Property<int>("Id").ValueGeneratedOnAdd();
            modelBuilder.Entity<User>().Property<int>("Id").ValueGeneratedOnAdd();
            modelBuilder.Entity<VerificationCode>()
            .HasOne(vc => vc.User)
            .WithMany(u => u.VerificationCodes)
            .HasForeignKey(vc => vc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, DateCreated = DateTime.Now, Name = "Admin", CreatedBy = "1" },
                new Role { Id = 2, DateCreated = DateTime.Now, Name = "Patient", CreatedBy = "1" }
                );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    DateCreated = DateTime.Now,
                    FirstName = "Hasbiy",
                    LastName = "Oyebo",
                    IsDeleted = false,
                    Email = "oyebohm@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin"),
                    RoleId = 1,
                    CreatedBy = "1"
                });

            modelBuilder.Entity<Profile>().HasData(
                new Profile
                {
                    Id = 1,
                    Weight = 45,
                    Gender = Core.Domain.Enum.Gender.Female,
                    DateOfBirth = new DateTime(2008, 03, 19),
                    Height = 90,
                    IsDeleted = false,
                    UserId = 1,
                    CreatedBy="1",
                    Nationality = "Nigerian",


                });


        }
    }
}
