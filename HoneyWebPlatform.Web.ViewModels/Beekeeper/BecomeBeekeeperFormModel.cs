namespace HoneyWebPlatform.Web.ViewModels.Beekeeper
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Beekeeper;

    public class BecomeBeekeeperFormModel
    {
        [Required]
        [StringLength(PhoneNumberMaxLength, MinimumLength = PhoneNumberMinLength)]
        [Phone]
        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; } = null!;
    }
}
