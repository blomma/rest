using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace rest.Models {
    public class AdContext : DbContext {
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        private static bool Created = false;

        public AdContext(DbContextOptions<AdContext> options)
            : base(options) {

            if (!Created) {
                Created = true;
                Database.Migrate();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder
                .UseLoggerFactory(MyLoggerFactory);

        public DbSet<Ad> Ads { get; set; }
    }
}