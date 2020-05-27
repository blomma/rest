using Microsoft.EntityFrameworkCore;

namespace rest.Models {
    public class AdContext : DbContext {
        private static bool Created = false;

        public AdContext(DbContextOptions<AdContext> options)
            : base(options) {

            if (!Created) {
                Created = true;
                Database.Migrate();
            }
        }

        public DbSet<Ad> Ads { get; set; }
    }
}