using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TransitPlannerWeb.Models
{
    public class AdminContextInitializer : DropCreateDatabaseIfModelChanges<AdminContext>
    {
        protected override void Seed(AdminContext db)
        {
            base.Seed(db);

            db.Settings.Add(new Setting { Key = "ALGORITHM", Value = "AStar" });
            db.Settings.Add(new Setting { Key = "GET_ON_OFF_TIME", Value = "1" });
            db.Settings.Add(new Setting { Key = "NORMAL_WALKING_SPEED", Value = "5" });
            db.Settings.Add(new Setting { Key = "FAST_WALKING_SPEED", Value = "8" });
            db.Settings.Add(new Setting { Key = "SLOW_WALKING_SPEED", Value = "3" });

            db.CoreServices.Add(new CoreService
            {
                BaseAddress = "http://localhost:18401/v1/RestfulService.svc",
                Weight = 5,
                Name = "Localhost Service",
                Description = "This is a service for localhost."
            });

            db.CoreServices.Add(new CoreService
            {
                BaseAddress = "http://localhost:18401/v1/RestfulService.svc",
                Weight = 2,
                Name = "Localhost 2 Service",
                Description = "This is a second service for localhost."
            });
        }
    }

    public class AdminContext : DbContext
    {
        public AdminContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer<AdminContext>(new AdminContextInitializer());
        }

        public DbSet<Setting> Settings { get; set; }
        public DbSet<CoreService> CoreServices { get; set; }
        public DbSet<TripDelay> TripDelays { get; set; }
        public DbSet<DisabledRoute> DisabledRoutes { get; set; }
        public DbSet<TroubleReport> TroubleReports { get; set; }
    }

    public class Setting
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class CoreService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BaseAddress { get; set; }
        public int Weight { get; set; }
    }

    public class TripDelay
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public int DelayInMinutes { get; set; }
        public DateTime When { get; set; }
    }

    public class DisabledRoute
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
    }

    public class TroubleReport
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Received { get; set; }
        public DateTime? FirstRead { get; set; }
        public int Category { get; set; }
        public int? StopId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Message { get; set; }
    }
}