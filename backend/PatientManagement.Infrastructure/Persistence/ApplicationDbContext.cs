using Microsoft.EntityFrameworkCore;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.MedicalRecordNumber).IsRequired().HasMaxLength(20);
            entity.HasIndex(p => p.MedicalRecordNumber).IsUnique();
            entity.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Gender).IsRequired().HasMaxLength(30);
            entity.Property(p => p.Phone).HasMaxLength(30);
            entity.Property(p => p.Email).HasMaxLength(200);
            entity.Property(p => p.Address).HasMaxLength(300);
            entity.Property(p => p.City).HasMaxLength(100);
            entity.Property(p => p.State).HasMaxLength(100);
            entity.Property(p => p.PostalCode).HasMaxLength(20);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        });

        base.OnModelCreating(modelBuilder);
    }
}
