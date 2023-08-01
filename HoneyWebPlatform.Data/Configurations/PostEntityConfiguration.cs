namespace HoneyWebPlatform.Data.Configurations
{
    using Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PostEntityConfiguration : IEntityTypeConfiguration<Post>
    {

        public void Configure(EntityTypeBuilder<Post> builder)
        {

            builder
                .Property(h => h.IsActive)
                .HasDefaultValue(true);

            builder
                .HasOne(h => h.User)
                .WithMany(a => a.OwnedPosts)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(SeedPosts());
        }

        private Post[] SeedPosts()
        {
            ICollection<Post> posts = new HashSet<Post>();

            Post post;

            post = new Post()
            {
                Id = 1,
                Title = "The site's first post",
                Content = "Welcome to this new site, I am the first beekeeper here" +
                          "enjoy your stay.",
                IsActive = true,
                UserId = Guid.Parse("29A205B5-19C1-4DBB-A318-0235F51AF7C7"),
            };

            posts.Add(post);

            return posts.ToArray();
        }
    }
}
