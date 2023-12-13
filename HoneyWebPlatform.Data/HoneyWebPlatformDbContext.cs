using HoneyWebPlatform.Data.Configurations;

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
        private readonly bool seedDb;
        public HoneyWebPlatformDbContext(DbContextOptions<HoneyWebPlatformDbContext> options, bool seedDb = true)
            : base(options)
        {
            this.seedDb = seedDb;
        }

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<Honey> Honeys { get; set; } = null!;

        public DbSet<Beekeeper> Beekeepers { get; set; } = null!;

        public DbSet<Propolis> Propolises { get; set; } = null!;

        public DbSet<Flavour> Flavours { get; set; } = null!;

        public DbSet<BeePollen> BeePollens { get; set; } = null!;

        public DbSet<Post> Posts { get; set; } = null!;

        public DbSet<Comment> Comments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //initdb
            builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
            //after creating two users through the app - continue with seeding these 4
            builder.ApplyConfiguration(new CategoryEntityConfiguration()); 
            builder.ApplyConfiguration(new FlavourEntityConfiguration());
            builder.ApplyConfiguration(new PostEntityConfiguration());
            builder.ApplyConfiguration(new BeePollenEntityConfigurator());
            //last seed these 3 since they need the 4 above first created
            builder.ApplyConfiguration(new HoneyEntityConfiguration());
            builder.ApplyConfiguration(new PropolisEntityConfiguration());
            builder.ApplyConfiguration(new CommentEntityConfiguration());

            //if (this.seedDb)
            //{
            //    //builder.ApplyConfiguration(new CategoryEntityConfiguration());
            //    builder.ApplyConfiguration(new HoneyEntityConfiguration());
            //    builder.ApplyConfiguration(new PropolisEntityConfiguration());
            //}

            //Assembly configAssembly = Assembly.GetAssembly(typeof(HoneyWebPlatformDbContext)) ??
            //                          Assembly.GetExecutingAssembly();
            
            //builder.ApplyConfigurationsFromAssembly(configAssembly);

            base.OnModelCreating(builder);
        }
    }
}