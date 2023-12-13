namespace HoneyWebPlatform.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static Common.EntityValidationConstants.Post;

    public class Comment
    {
        public Comment()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsActive { get; set; }

        [Required]
        [MaxLength(ContentMaxLength)]
        public string Content { get; set; } = null!; 

        public Guid ParentPostId { get; set; }

        public virtual Post ParentPost { get; set; } = null!;

        public Guid AuthorId { get; set; }
        
        public virtual ApplicationUser Author { get; set; } = null!;
    }
}