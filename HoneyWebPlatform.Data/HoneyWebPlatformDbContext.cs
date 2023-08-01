namespace HoneyWebPlatform.Data
{
    using System.Collections.Generic;
    using System.Reflection;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    using Models;

    public class HoneyWebPlatformDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public HoneyWebPlatformDbContext(DbContextOptions<HoneyWebPlatformDbContext> options)
        : base(options)
        { }

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<Honey> Honeys { get; set; } = null!;

        public DbSet<Beekeeper> Beekeepers { get; set; } = null!;

        public DbSet<Propolis> Propolises { get; set; } = null!;

        public DbSet<Flavour> Flavours { get; set; } = null!;

        public DbSet<BeePollen> BeePollens { get; set; } = null!;

        public DbSet<Post> Posts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            Assembly configAssembly = Assembly.GetAssembly(typeof(HoneyWebPlatformDbContext)) ??
                                      Assembly.GetExecutingAssembly();
            builder.ApplyConfigurationsFromAssembly(configAssembly);

            base.OnModelCreating(builder);
        }
    }
}