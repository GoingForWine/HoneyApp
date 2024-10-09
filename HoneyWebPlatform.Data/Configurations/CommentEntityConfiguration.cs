namespace HoneyWebPlatform.Data.Configurations
{
    using Models;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CommentEntityConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder
                .HasOne(c => c.Author)
                .WithMany(a => a.OwnedComments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(c => c.ParentPost)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.ParentPostId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.HasData(SeedComments());
        }

        private Comment[] SeedComments()
        {
            ICollection<Comment> comments = new HashSet<Comment>();

            Comment comment;

            comment = new Comment()
            {
                Content = "This is the first comment on the first post!",
                CreatedOn = DateTime.Now,
                IsActive = true,
                ParentPostId = Guid.Parse("7B55A828-68BE-45E8-9991-0F19CEE32622"),
                AuthorId = Guid.Parse("BD56FE08-BD10-4384-89BE-63A211FBBC61")
            };

            comments.Add(comment);

            return comments.ToArray();
        }
    }

}
