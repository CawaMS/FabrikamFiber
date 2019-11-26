namespace FabrikamFiber.DAL.Data
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public class FabrikamFiberWebContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Removing statement below due to IncludeMetadataConvention Obsoletion Error
            //modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
            base.OnModelCreating(modelBuilder);
        }
        public FabrikamFiberWebContext()
            : base("FabrikamFiber-Express")
        {
        }

        public FabrikamFiberWebContext(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<Models.Customer> Customers { get; set; }

        public DbSet<Models.Employee> Employees { get; set; }

        public DbSet<Models.ServiceTicket> ServiceTickets { get; set; }

        public DbSet<Models.ServiceLogEntry> ServiceLogEntries { get; set; }

        public DbSet<Models.Message> Messages { get; set; }

        public DbSet<Models.Alert> Alerts { get; set; }

        public DbSet<Models.ScheduleItem> ScheduleItems { get; set; }
    }
}