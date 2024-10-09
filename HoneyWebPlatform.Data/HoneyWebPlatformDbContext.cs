namespace HoneyWebPlatform.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    using Models;
    using Configurations;

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

        public DbSet<SubscribedEmail> SubscribedEmails { get; set; } = null!;

        public DbSet<Cart> Carts { get; set; } = null!;

        public DbSet<CartItem> CartItems { get; set; } = null!;

        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;



        protected override void OnModelCreating(ModelBuilder builder)
        {
            //initdb
            var adminRoleId = Guid.NewGuid();
            builder.Entity<IdentityRole<Guid>>().HasData(new IdentityRole<Guid>
            {
                Id = adminRoleId,
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });

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


            builder.ApplyConfiguration(new CartEntityConfiguration());

            builder.ApplyConfiguration(new OrderEntityConfiguration());

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