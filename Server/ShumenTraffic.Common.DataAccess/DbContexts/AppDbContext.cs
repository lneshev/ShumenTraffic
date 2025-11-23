using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Common.Core.Entities;
using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Common.Core.Entities.BusStops;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Entities.TransportationCompanies;
using ShumenTraffic.Common.Core.Entities.Zones;
using ShumenTraffic.Common.Core.Resources;
using ShumenTraffic.Common.DataAccess.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.DataAccess.DbContexts
{
    /// <summary>
    /// Entity Framework Core DbContext for ShumenTraffic application.
    /// Integrates ASP.NET Core Identity for user authentication and authorization.
    /// </summary>
    public class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        /// <summary>
        /// Initializes a new instance of the AppDbContext class.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<TransportationCompany> TransportationCompanies { get; set; }
        public DbSet<BusLine> BusLines { get; set; }
        public DbSet<TransportationCompanyBusLine> TransportationCompanyBusLines { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<BusStop> BusStops { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteStop> RouteStops { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleCourse> ScheduleCourses { get; set; }

        /// <summary>
        /// Configures the model and relationships.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set Cascade Delete Behavior to Restrict except for entities in tableWithDeleteCascade
            string[] tableWithDeleteCascade = new string[] { };

            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade && !tableWithDeleteCascade.Contains(fk.DeclaringEntityType.Name.Split('.').Last()));

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);

            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CS_AS");

            // TransportationCompany configuration
            modelBuilder.Entity<TransportationCompany>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<TransportationCompany>()
                .HasMany(x => x.TransportationCompanyBusLines)
                .WithOne(x => x.TransportationCompany)
                .HasForeignKey(x => x.TransportationCompanyId);

            // BusLine configuration
            modelBuilder.Entity<BusLine>()
                .HasMany(x => x.TransportationCompanyBusLines)
                .WithOne(x => x.BusLine)
                .HasForeignKey(x => x.BusLineId);
            modelBuilder.Entity<BusLine>()
                .HasMany(x => x.Routes)
                .WithOne(x => x.BusLine)
                .HasForeignKey(x => x.BusLineId);

            // Zone configuration
            modelBuilder.Entity<Zone>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<Zone>()
                .HasMany(x => x.BusStops)
                .WithOne(x => x.Zone)
                .HasForeignKey(x => x.ZoneId);

            // BusStop configuration
            modelBuilder.Entity<BusStop>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<BusStop>()
                .Property(x => x.Location)
                .HasColumnType("geography")
                .HasScale(6);
            modelBuilder.Entity<BusStop>()
                .HasIndex(x => x.ZoneId);
            modelBuilder.Entity<BusStop>()
                .HasMany(x => x.RouteStops)
                .WithOne(x => x.BusStop)
                .HasForeignKey(x => x.BusStopId);

            // Route configuration
            modelBuilder.Entity<Route>()
                .HasIndex(x => new { x.Name, x.BusLineId, x.Direction })
                .IsUnique();
            modelBuilder.Entity<Route>()
                .HasIndex(x => x.BusLineId);
            modelBuilder.Entity<Route>()
                .HasMany(x => x.RouteStops)
                .WithOne(x => x.Route)
                .HasForeignKey(x => x.RouteId);
            modelBuilder.Entity<Route>()
                .HasMany(x => x.ScheduleCourses)
                .WithOne(x => x.Route)
                .HasForeignKey(x => x.RouteId);

            // RouteStop configuration
            modelBuilder.Entity<RouteStop>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<RouteStop>()
                .Property(x => x.Latitude)
                .HasPrecision(10, 8);
            modelBuilder.Entity<RouteStop>()
                .Property(x => x.Longitude)
                .HasPrecision(11, 8);
            modelBuilder.Entity<RouteStop>()
                .HasIndex(x => new { x.RouteId, x.StopOrder });
            modelBuilder.Entity<RouteStop>()
                .HasIndex(x => x.BusStopId);
            modelBuilder.Entity<RouteStop>()
                .HasOne(x => x.BusStop)
                .WithMany(x => x.RouteStops)
                .HasForeignKey(x => x.BusStopId)
                .IsRequired(false); // BusStopId is nullable

            // Schedule configuration
            modelBuilder.Entity<Schedule>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<Schedule>()
                .Property(x => x.DayType)
                .IsRequired()
                .HasMaxLength(20);
            modelBuilder.Entity<Schedule>()
                .HasMany(x => x.ScheduleCourses)
                .WithOne(x => x.Schedule)
                .HasForeignKey(x => x.ScheduleId);

            // ScheduleCourse configuration
            modelBuilder.Entity<ScheduleCourse>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<ScheduleCourse>()
                .HasIndex(x => x.ScheduleId);
            modelBuilder.Entity<ScheduleCourse>()
                .HasIndex(x => x.RouteId);

            // TransportationCompanyBusLine configuration (junction table)
            modelBuilder.Entity<TransportationCompanyBusLine>()
                .HasKey(x => new { x.TransportationCompanyId, x.BusLineId });
            modelBuilder.Entity<TransportationCompanyBusLine>()
                .HasIndex(x => x.TransportationCompanyId);
            modelBuilder.Entity<TransportationCompanyBusLine>()
                .HasIndex(x => x.BusLineId);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            if (Database.CurrentTransaction == null)
            {
                throw new InvalidOperationException(Strings.SavingDataToDBWithoutATransactionIsNotAllowed);
            }

            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.State != EntityState.Unchanged)
                {
                    if (entry.Entity is ITrackableEntityBase trackable)
                    {
                        switch (entry.State)
                        {
                            case EntityState.Added:
                                trackable.CreatedAt = trackable.CreatedAt == DateTimeOffset.MinValue ? DateTimeOffset.UtcNow : trackable.CreatedAt;
                                break;
                            case EntityState.Modified:
                                trackable.UpdatedAt = DateTimeOffset.UtcNow;
                                break;
                            case EntityState.Deleted:
                                trackable.UpdatedAt = DateTimeOffset.UtcNow;
                                break;
                        }
                    }

                    if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                    {
                        var validationContext = new ValidationContext(entry.Entity);
                        Validator.ValidateObject(entry.Entity, validationContext, true);
                    }
                }
            }
        }
    }
}