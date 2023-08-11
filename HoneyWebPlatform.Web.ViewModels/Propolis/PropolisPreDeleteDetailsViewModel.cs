namespace HoneyWebPlatform.Web.ViewModels.Propolis
{
    using System.ComponentModel.DataAnnotations;

    public class PropolisPreDeleteDetailsViewModel
    {
        public string Title { get; set; } = null!;

        [Display(Name = "Image Link")]
        public string ImageUrl { get; set; } = null!;
    }
}
