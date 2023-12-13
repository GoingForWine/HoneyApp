namespace HoneyWebPlatform.Web.ViewModels.Blog
{
    using System.ComponentModel.DataAnnotations;

    public class PostPreDeleteDetailsViewModel
    {
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        [Display(Name = "Image Link")]
        public string ImageUrl { get; set; } = null!;
    }
}
