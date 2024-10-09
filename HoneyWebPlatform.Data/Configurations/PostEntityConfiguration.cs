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
                .Property(h => h.CreatedOn)
                .HasDefaultValueSql("GETDATE()");

            builder
                .HasOne(h => h.Author)
                .WithMany(a => a.OwnedPosts)
                .HasForeignKey(h => h.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.HasData(SeedPosts());
        }

        private Post[] SeedPosts()
        {
            ICollection<Post> posts = new HashSet<Post>();

            Post post;

            post = new Post()
            {
                Title = "The site's first post",
                Content = "Welcome to this new site, I am the first beekeeper here" +
                          "enjoy your stay.",
                IsActive = true,
                AuthorId = Guid.Parse("BD56FE08-BD10-4384-89BE-63A211FBBC61"),
                ImageUrl = "https://th.bing.com/th/id/OIP.eYhgoQcmVrOQG4mTZWpdLwHaE6?rs=1&pid=ImgDetMain",
            };

            posts.Add(post);

            return posts.ToArray();
        }
    }
}
