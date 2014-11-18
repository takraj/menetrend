using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TransitPlannerWeb.Models
{
    public class AdminContext : DbContext
    {
        public AdminContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer<AdminContext>(new CreateDatabaseIfNotExists<AdminContext>());
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