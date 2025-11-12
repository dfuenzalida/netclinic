using Microsoft.EntityFrameworkCore;
using NetClinic.Api.Models;

namespace NetClinic.Api.Data;

public class NetClinicDbContext : DbContext
{
    public NetClinicDbContext(DbContextOptions<NetClinicDbContext> options) : base(options)
    {
    }

    public DbSet<Veterinarian> Veterinarians { get; set; }
    public DbSet<Specialty> Specialties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the many-to-many relationship between Veterinarian and Specialty
        modelBuilder.Entity<Veterinarian>()
            .HasMany(v => v.Specialties)
            .WithMany(s => s.Veterinarians)
            .UsingEntity<Dictionary<string, object>>(
                "vet_specialties",
                j => j.HasOne<Specialty>().WithMany().HasForeignKey("specialty_id"),
                j => j.HasOne<Veterinarian>().WithMany().HasForeignKey("vet_id"));

        // Configure the Veterinarian entity
        modelBuilder.Entity<Veterinarian>(entity =>
        {
            entity.HasKey(v => v.Id);
        });

        // Configure the Specialty entity
        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(s => s.Id);
        });
    }
}