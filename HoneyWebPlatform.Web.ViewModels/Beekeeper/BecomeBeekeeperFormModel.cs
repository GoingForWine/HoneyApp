namespace HoneyWebPlatform.Web.ViewModels.Beekeeper
{
    using Common;
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Beekeeper;

    public class BecomeBeekeeperFormModel
    {
        [Required(ErrorMessage = "Моля, въведете вашият телефонен номер.")]
        [StringLength(PhoneNumberMaxLength, MinimumLength = PhoneNumberMinLength)]
        [Phone]
        [Display(Name = "Телефонен номер")]
        public string PhoneNumber { get; set; } = null!;

        // New property for hive pic
        [Display(Name = "Снимка на кошер")]
        [MaxFileSize(ProfilePictureMaxSize, ErrorMessage = "Максималният размер на файла за профилната снимка е 2 мегабайта.")]
        public IFormFile? HivePicture { get; set; }

        public string HivePicturePath { get; set; }
    }
}
