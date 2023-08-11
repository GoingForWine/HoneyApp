namespace HoneyWebPlatform.Web.ViewModels.Propolis
{
    using System.ComponentModel.DataAnnotations;

    public class PropolisAllViewModel
    {
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;
        
        [Display(Name = "Image Link")]
        public string ImageUrl { get; set; } = null!;

        [Display(Name = "Price")]
        public decimal Price { get; set; }
    }
}