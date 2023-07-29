namespace HoneyPlatform.Data
{
    using System.Reflection;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    using Models;

    public class HoneyPlatformDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public HoneyPlatformDbContext(DbContextOptions<HoneyPlatformDbContext> options)
            : base(options)
        { }

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<Honey> Honeys { get; set; } = null!;

        public DbSet<Beekeeper> Beekeepers { get; set; } = null!;

        public DbSet<Propolis> Propolises { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            Assembly configAssembly = Assembly.GetAssembly(typeof(HoneyPlatformDbContext)) ??
                                      Assembly.GetExecutingAssembly();
            builder.ApplyConfigurationsFromAssembly(configAssembly);

            base.OnModelCreating(builder);
        }
    }
}