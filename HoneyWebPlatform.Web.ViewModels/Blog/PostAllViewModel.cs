namespace HoneyWebPlatform.Web.ViewModels.Blog
{
    using Data.Models;
    using System.ComponentModel.DataAnnotations;

    public class PostAllViewModel
    {
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        [Display(Name = "Image Link")]
        public string ImageUrl { get; set; } = null!;

        public bool IsActive { get; set; }

        public int CommentsCount { get; set; }

        public ApplicationUser Author { get; set; } = null!;
    }
}
