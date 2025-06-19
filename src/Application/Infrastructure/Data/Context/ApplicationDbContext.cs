using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Application.Domain.Entities;
using DayOfWeek = Application.Domain.Entities.DayOfWeek;

namespace Application.Infrastructure.Data.Context;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<
    ApplicationUser,
    ApplicationRole, 
    long,
    ApplicationUserClaim,
    ApplicationUserRole,
    ApplicationUserLogin,
    ApplicationRoleClaim,
    ApplicationUserToken>
    (options)
{

    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<VenueCategory> VenueCategories => Set<VenueCategory>();
    public DbSet<BusinessHours> BusinessHours => Set<BusinessHours>();
    public DbSet<DayOfWeek> DayOfWeeks => Set<DayOfWeek>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasPostgresExtension("address_standardizer");
        builder.HasPostgresExtension("address_standardizer_data_us");
        builder.HasPostgresExtension("fuzzystrmatch");
        builder.HasPostgresExtension("plpgsql");
        builder.HasPostgresExtension("postgis");
        builder.HasPostgresExtension("postgis_raster");
        builder.HasPostgresExtension("postgis_sfcgal");
        builder.HasPostgresExtension("postgis_tiger_geocoder");
        builder.HasPostgresExtension("postgis_topology");

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
