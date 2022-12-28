
using Microsoft.EntityFrameworkCore;
using Xarajat.Bot.Entities;

#pragma warning disable CS8618
namespace Xarajat.Bot.Context
{
    public class XarajatDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Outlay> Outlays { get; set; }

        public XarajatDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //with configuration class
            OutLaysConfiguration.Configure(modelBuilder.Entity<Outlay>());

            //with configuration class
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(XarajatDbContext).Assembly);
        }
       
    }
}