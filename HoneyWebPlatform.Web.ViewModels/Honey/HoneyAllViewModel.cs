namespace HoneyWebPlatform.Web.ViewModels.Honey
{
    using Services.Mapping;
    using System.ComponentModel.DataAnnotations;
    using Data.Models;

    public class HoneyAllViewModel : IMapFrom<Honey>
    {
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Origin { get; set; } = null!;

        [Display(Name = "Image Link")]
        public string ImageUrl { get; set; } = null!;

        [Display(Name = "Price")]
        public decimal Price { get; set; }
    }
}