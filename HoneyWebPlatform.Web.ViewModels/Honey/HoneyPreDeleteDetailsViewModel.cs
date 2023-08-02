namespace HoneyWebPlatform.Web.ViewModels.Honey
{
    using System.ComponentModel.DataAnnotations;

    public class HoneyPreDeleteDetailsViewModel
    {
        public string Title { get; set; } = null!;

        public string Origin { get; set; } = null!;

        [Display(Name = "Image Link")]
        public string ImageUrl { get; set; } = null!;
    }
}
