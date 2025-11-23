using Microsoft.EntityFrameworkCore;
using NetClinic.Api.Models;

namespace NetClinic.Api.Data;

public class NetClinicDbContext : DbContext
{
    public NetClinicDbContext(DbContextOptions<NetClinicDbContext> options) : base(options)
    {
    }

    public DbSet<Veterinarian> Veterinarians { get; set; }
    public DbSet<Owner> Owners { get; set; }
    public DbSet<Pet> Pets { get; set; }

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

        modelBuilder.Entity<Owner>()
            .HasMany(o => o.Pets)
            .WithOne(p => p.Owner)
            .HasForeignKey(p => p.OwnerId);

        modelBuilder.Entity<Pet>()
            .HasMany(p => p.Visits)
            .WithOne(v => v.Pet)
            .HasForeignKey(v => v.PetId);

        modelBuilder.Entity<Pet>()
            .HasOne(p => p.PetType)
            .WithMany()
            .HasForeignKey(p => p.TypeId);

    }
}