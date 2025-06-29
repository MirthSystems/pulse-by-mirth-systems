using Microsoft.EntityFrameworkCore;
using Application.Domain.Entities;

namespace Application.Infrastructure.Data.Context;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{    
    public DbSet<VenueEntity> Venues => Set<VenueEntity>();
    public DbSet<VenueCategoryEntity> VenueCategories => Set<VenueCategoryEntity>();
    public DbSet<BusinessHoursEntity> BusinessHours => Set<BusinessHoursEntity>();
    public DbSet<DayOfWeekEntity> DaysOfWeek => Set<DayOfWeekEntity>();
    public DbSet<SpecialEntity> Specials => Set<SpecialEntity>();
    public DbSet<SpecialCategoryEntity> SpecialCategories => Set<SpecialCategoryEntity>();
    
    // Authorization entities
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<UserVenuePermissionEntity> UserVenuePermissions => Set<UserVenuePermissionEntity>();
    public DbSet<VenueInvitationEntity> VenueInvitations => Set<VenueInvitationEntity>();

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
