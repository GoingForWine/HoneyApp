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

        [Required(ErrorMessage = "Моля напишете наименованието.")]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength,
            ErrorMessage = "Наименованието трябва да е текст с поне {2} символа и най-много {1}.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Моля опишете прополиса.")]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength,
            ErrorMessage = "Описанието трябва да е текст с поне {2} символа и най-много {1}.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Моля добавете линк към снимка.")]
        [StringLength(ImageUrlMaxLength)]
        [Display(Name = "Линк към снимка")]
        public string ImageUrl { get; set; } = null!;

        [Required(ErrorMessage = "Полето Цена е задължително.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$",
            ErrorMessage = "Цената трябва да бъде число и може да включва до две десетични цифри, разделени с точка.")]
        [Range(typeof(decimal), PriceMinValue, PriceMaxValue,
            ErrorMessage = "Цената трябва да е между 2 и 10 лева.")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Display(Name = "Вкус")]
        public int FlavourId { get; set; }

        public IEnumerable<PropolisSelectFlavourFormModel> Flavours { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<PropolisFormModel, Propolis>()
                .ForMember(d => d.BeekeeperId, opt => opt.Ignore());
        }
    }
}