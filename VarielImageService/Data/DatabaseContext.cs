using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Variel.ImageService.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Application> Applications { get; set; }
        public DbSet<MasterImage> MasterImages { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
