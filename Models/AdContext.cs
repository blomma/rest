using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace rest.Models {
    public class AdContext : DbContext {
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public AdContext(DbContextOptions<AdContext> options)
            : base(options) {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder
                .UseLoggerFactory(MyLoggerFactory);

        public DbSet<Ad> Ads { get; set; }
    }
}