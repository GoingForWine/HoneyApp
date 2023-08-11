namespace HoneyWebPlatform.Web.ViewModels.Propolis
{
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    
    using Data.Models;
    using Flavour;
    using Services.Mapping;

    using static Common.EntityValidationConstants.Propolis;

    public class PropolisFormModel : IMapTo<Propolis>, IHaveCustomMappings
    {
        public PropolisFormModel()
        {
            Flavours = new HashSet<PropolisSelectFlavourFormModel>();
        }

        [Required]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength)]
        public string Title { get; set; } = null!;
        
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

        [Display(Name = "Flavour")]
        public int FlavourId { get; set; }

        public IEnumerable<PropolisSelectFlavourFormModel> Flavours { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<PropolisFormModel, Propolis>()
                .ForMember(d => d.BeekeeperId, opt => opt.Ignore());
        }
    }
}