namespace HoneyWebPlatform.Web.ViewModels.Honey
{
    using System.ComponentModel.DataAnnotations;

    public class HoneyAllViewModel
    {
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Address { get; set; } = null!;

        [Display(Name = "Image Link")]
        public string ImageUrl { get; set; } = null!;

        [Display(Name = "Price")]
        public decimal Price { get; set; }
    }
}