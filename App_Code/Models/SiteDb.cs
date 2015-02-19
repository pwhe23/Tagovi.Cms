using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace Site
{
    public class SiteDb : DbContext
    {
        public SiteDb()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SiteDb, SiteDbMigrationConfiguration>());
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.ValidateOnSaveEnabled = true;
            Database.Log = x => System.Diagnostics.Debug.Write(x);
        }

        public virtual DbSet<Part> Parts { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<User> Users { get; set; }

        internal class SiteDbMigrationConfiguration : DbMigrationsConfiguration<SiteDb>
        {
            public SiteDbMigrationConfiguration()
            {
                AutomaticMigrationsEnabled = true;
                AutomaticMigrationDataLossAllowed = false;
            }

            protected override void Seed(SiteDb context)
            {
                context.Users.AddOrUpdate(x => x.Email, new[]
                {
                    new User{ Email = "paul@tagovi.com", Password = "pw", IsAdmin = true},
                });
            }
        };
    };
}
