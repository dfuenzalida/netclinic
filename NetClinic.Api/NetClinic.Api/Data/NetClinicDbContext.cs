using Microsoft.EntityFrameworkCore;
using NetClinic.Api.Services;

namespace NetClinic.Api.Data;

public class NetClinicDbContext : DbContext
{
    public NetClinicDbContext(DbContextOptions<NetClinicDbContext> options) : base(options)
    {
    }

    public DbSet<Veterinarian> Veterinarians { get; set; }

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);

    //     // Configure the Veterinarian entity
    //     modelBuilder.Entity<Veterinarian>(entity =>
    //     {
    //         entity.HasKey(v => v.Id);
    //         entity.Property(v => v.FirstName).IsRequired().HasMaxLength(100);
    //         entity.Property(v => v.LastName).IsRequired().HasMaxLength(100);
    //     });

    //     // Seed initial data
    //     modelBuilder.Entity<Veterinarian>().HasData(
    //         new Veterinarian(1, "Sarah", "Johnson"),
    //         new Veterinarian(2, "Michael", "Chen"),
    //         new Veterinarian(3, "Emily", "Rodriguez"),
    //         new Veterinarian(4, "David", "Thompson"),
    //         new Veterinarian(5, "Lisa", "Anderson"),
    //         new Veterinarian(6, "Robert", "Wilson")
    //     );
    // }
}