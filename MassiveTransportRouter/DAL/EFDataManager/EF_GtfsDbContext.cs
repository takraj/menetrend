using MTR.DataAccess.EFDataManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.DataAccess.EFDataManager
{
    class EF_GtfsDbContext : DbContext
    {
        public DbSet<EF_Agency> Agencies { get; set; }
        public DbSet<EF_Route> Routes { get; set; }
        public DbSet<EF_Calendar> Calendars { get; set; }
        public DbSet<EF_CalendarDate> CalendarDates { get; set; }
        public DbSet<EF_Stop> Stops { get; set; }
        public DbSet<EF_Shape> Shapes { get; set; }
        public DbSet<EF_Trip> Trips { get; set; }
        public DbSet<EF_StopTime> StopTimes { get; set; }
        public DbSet<EF_StopRouteStopEdge> StopEdges { get; set; }

        public static string ConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
        }

        public EF_GtfsDbContext()
            : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
