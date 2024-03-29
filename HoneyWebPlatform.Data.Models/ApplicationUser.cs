﻿namespace HoneyWebPlatform.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Identity;

    using static Common.EntityValidationConstants.User;

    /// <summary>
    /// This is custom user class that works with the default ASP.NET Core Identity.
    /// You can add additional info to the built-in users.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid();
            OwnedPosts = new HashSet<Post>();
            OwnedComments = new HashSet<Comment>();
        }

        [Required]
        [MaxLength(FirstNameMaxLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(LastNameMaxLength)]
        public string LastName { get; set; } = null!;


        [MaxLength(255)] 
        public string? ProfilePicturePath { get; set; }

        public virtual ICollection<Post> OwnedPosts { get; set; }

        public virtual ICollection<Comment> OwnedComments { get; set; }

        public virtual bool IsSubscribed { get; set; }
    }
}