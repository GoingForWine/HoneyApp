namespace HoneyWebPlatform.Web.ViewModels.Honey
{
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;

    using Common;
    using Category;
    using Data.Models;
    using Microsoft.AspNetCore.Http;
    using Services.Mapping;

    using static Common.EntityValidationConstants.Honey;

    public class HoneyFormModel : IMapTo<Honey>, IHaveCustomMappings
    {
        public HoneyFormModel()
        {
            Categories = new HashSet<HoneySelectCategoryFormModel>();
        }

        [Required(ErrorMessage = "Моля напишете наименованието.")]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength, 
            ErrorMessage = "Наименованието трябва да е текст с поне {2} символа и най-много {1}.")]

        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Моля напишете произхода.")]
        [StringLength(OriginMaxLength, MinimumLength = OriginMinLength,
            ErrorMessage = "Произходът трябва да е текст с поне {2} символа и най-много {1}.")]
        public string Origin { get; set; } = null!;

        [Required(ErrorMessage = "Моля опишете меда.")]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength,
            ErrorMessage = "Описанието трябва да е текст с поне {2} символа и най-много {1}.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Моля добавете снимка до 2MB.")]
        [Display(Name = "Снимка")]
        [MaxFileSize(HoneyPictureMaxSize, ErrorMessage = "Максималният размер на файла за снимка е 2 мегабайта.")]
        public IFormFile? HoneyPicture { get; set; }

        public string HoneyPicturePath { get; set; }

        [Required(ErrorMessage = "Полето Цена е задължително.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", 
            ErrorMessage = "Цената трябва да бъде число и може да включва до две десетични цифри, разделени с точка.")]
        [Range(typeof(decimal), PriceMinValue, PriceMaxValue, 
            ErrorMessage = "Цената трябва да е между 5 и 25 лева.")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        public IEnumerable<HoneySelectCategoryFormModel> Categories { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<HoneyFormModel, Honey>()
                .ForMember(d => d.BeekeeperId, opt => opt.Ignore());
        }
    }
}