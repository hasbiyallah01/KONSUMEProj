using DaticianProj.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Project.Models.Entities;
using System.Text.Json;

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
            modelBuilder.Entity<Profile>().Property<int>("Id").ValueGeneratedOnAdd();
            modelBuilder.Entity<UserInteraction>().Property<int>("Id").ValueGeneratedOnAdd();
            modelBuilder.Entity<VerificationCode>().Property<int>("Id").ValueGeneratedOnAdd();
            modelBuilder.Entity<VerificationCode>()
            .HasOne(vc => vc.User)
            .WithMany(u => u.VerificationCodes)
            .HasForeignKey(vc => vc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Profile>()
            .Property(p => p.AllergiesSerialized)
            .HasConversion(
                v => v,
                v => v
            );

            modelBuilder.Entity<Profile>()
            .Property(p => p.GoalsSerialized)
            .HasConversion(
                v => v,
                v => v
            );

            modelBuilder.Entity<Profile>().Ignore(p => p.Allergies);
            modelBuilder.Entity<Profile>().Ignore(p => p.UserGoals);

            modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, DateCreated = DateTime.UtcNow, Name = "Admin", CreatedBy = "1" },
            new Role { Id = 2, DateCreated = DateTime.UtcNow, Name = "Patient", CreatedBy = "1" }
            );

            modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                DateCreated = DateTime.UtcNow,
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
                DateCreated = DateTime.UtcNow,
                Height = 90,
                IsDeleted = false,
                UserId = 1,
                CreatedBy = "1",
                Nationality = "Nigerian",
                AllergiesSerialized = JsonSerializer.Serialize(new List<string>()),
                GoalsSerialized = JsonSerializer.Serialize(new List<string>())
            });
        }
    }
}
