using AppCore.Enums;
using AppCore.Interfaces;
using Infrastructure.Entities;
using AppCore.Models;
using AppCore.ValueObjects;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Infrastructure.Security;
namespace Infrastructure.Context;

public class ContactsDbContext : IdentityDbContext<CrmUser, CrmRole, string>
{
    public DbSet<Person> People { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Organization> Organizations { get; set; }

    public ContactsDbContext() { }
    

    public ContactsDbContext(DbContextOptions<ContactsDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Pointing to the specific folder to keep things organized
            optionsBuilder.UseSqlite("Data Source=contacts.db");
        }
        
        // This stops EF from panicking about the dynamic PasswordHash salt
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); 

        // 1. Identity Configuration
        builder.Entity<CrmUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.Property(u => u.Department).HasMaxLength(100);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        builder.Entity<CrmRole>(entity =>
        {
            entity.Property(r => r.Name).HasMaxLength(50);
        });

        // 2. TPH (Table Per Hierarchy) Configuration
        builder.Entity<Contact>()
            .HasDiscriminator<string>("ContactType")
            .HasValue<Person>("Person")
            .HasValue<Company>("Company")
            .HasValue<Organization>("Organization");

        builder.Entity<Contact>(entity =>
        {
            entity.Property(p => p.Email).HasMaxLength(200);
            entity.Property(p => p.Phone).HasMaxLength(20);
            
            entity.OwnsOne(c => c.Address, a => {
                a.Property(ad => ad.City).HasMaxLength(100);
                a.Property(ad => ad.Street).HasMaxLength(200);
            });
        });

        // 3. Person Configuration
        builder.Entity<Person>(entity =>
        {
            entity.Property(p => p.BirthDate).HasColumnType("date");
            entity.Property(p => p.Gender).HasConversion<string>();
            entity.Property(p => p.Status).HasConversion<string>();
        });

        // 4. Global Relationships
        builder.Entity<Person>()
            .HasOne(p => p.Employer)
            .WithMany(e => e.Employees)
            .HasForeignKey(p => p.EmployerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Organization>()
            .HasMany(o => o.Members)
            .WithOne(p => p.Organization)
            .HasForeignKey(p => p.OrganizationId);

        // 5. Seed Data (Hierarchical Order)
        SeedData(builder);
    }

    private void SeedData(ModelBuilder builder)
    {
        var companyId = Guid.Parse("516A34D7-CCFB-4F20-85F3-62BD0F3AF271");
        var personId = Guid.Parse("3D54091D-ABC8-49EC-9590-93AD3ED5458F");
        var adminRoleId = Guid.Parse("A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D").ToString();
        var adminUserId = Guid.Parse("B2C3D4E5-F6A7-4B5C-9D8E-1F2A3B4C5D6E").ToString();
        
        var staticDate = new DateTime(2024, 1, 1);

        // STEP A: Seed Roles (No dependencies)
        builder.Entity<CrmRole>().HasData(new CrmRole
        {
            Id = adminRoleId,
            Name = "Administrator",
            NormalizedName = "ADMINISTRATOR",
            Description = "System Administrator"
        });

        // STEP B + D: Seed Contact (Company + Person razem!)
        builder.Entity<Person>().HasData(new Person
        {
            Id = personId,
            FirstName = "Adam",
            LastName = "Nowak",
            Email = "adam@wsei.edu.pl",
            Phone = "123456789",
            Gender = Gender.Male,
            Status = ContactStatus.Active,
            BirthDate = new DateTime(2001, 1, 11),
            EmployerId = companyId,
            CreatedAt = staticDate,
            UpdatedAt = staticDate
        });
        builder.Entity<Company>().HasData(new Company
        {
            Id = companyId,
            Email = "biuro@wsei.edu.pl",
            Phone = "123567123",
            Status = ContactStatus.Active,
            CreatedAt = staticDate
        });

        // STEP C: Seed Admin User
        builder.Entity<CrmUser>().HasData(new CrmUser
        {
            Id = adminUserId,
            UserName = "admin@wsei.edu.pl",
            NormalizedUserName = "ADMIN@WSEI.EDU.PL",
            Email = "admin@wsei.edu.pl",
            NormalizedEmail = "ADMIN@WSEI.EDU.PL",
            FirstName = "Admin",
            LastName = "User",
            FullName = "Admin User",
            Department = "IT",
            Status = SystemUserStatus.Active,
            CreatedAt = staticDate,
            EmailConfirmed = true,
            PasswordHash = new PasswordHasher<CrmUser>().HashPassword(null!, "Admin123!")
        });

        // STEP D: Seed Person (Child - depends on CompanyId)
       
    }
}