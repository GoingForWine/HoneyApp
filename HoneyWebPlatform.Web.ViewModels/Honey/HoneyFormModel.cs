namespace HoneyWebPlatform.Web.ViewModels.Honey
{
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;

    using Category;
    using Data.Models;
    using Services.Mapping;

    using static Common.EntityValidationConstants.Honey;

    public class HoneyFormModel : IMapTo<Honey>, IHaveCustomMappings
    {
        public HoneyFormModel()
        {
            Categories = new HashSet<HoneySelectCategoryFormModel>();
        }

        [Required]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(OriginMaxLength, MinimumLength = OriginMinLength)]
        public string Origin { get; set; } = null!;

        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength)]
        public string Description { get; set; } = null!;

        [Required]
        [StringLength(ImageUrlMaxLength)]
        [Display(Name = "Image Link")]
        public string ImageUrl { get; set; } = null!;

        [Range(typeof(decimal), PriceMinValue, PriceMaxValue)]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public IEnumerable<HoneySelectCategoryFormModel> Categories { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<HoneyFormModel, Honey>()
                .ForMember(d => d.BeekeeperId, opt => opt.Ignore());
        }
    }
}