namespace HoneyWebPlatform.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Post;

    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(TitleMaxLength, MinimumLength = 10)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(ContentMaxLength, MinimumLength = 30)]
        public string Content { get; set; } = null!;

        public bool IsActive { get; set; }

        [Required]
        public Guid UserId { get; set; }
        
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
