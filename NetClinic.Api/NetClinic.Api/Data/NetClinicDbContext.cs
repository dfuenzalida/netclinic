using Microsoft.EntityFrameworkCore;
using NetClinic.Api.Models;

namespace NetClinic.Api.Data;

public class NetClinicDbContext : DbContext
{
    public NetClinicDbContext(DbContextOptions<NetClinicDbContext> options) : base(options)
    {
    }

    public DbSet<Veterinarian> Veterinarians { get; set; }

}