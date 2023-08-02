namespace HoneyWebPlatform.Web.ViewModels.Beekeeper
{
    using System.ComponentModel.DataAnnotations;

    public class BeekeeperInfoOnHoneyViewModel
    {
        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; } = null!;
    }
}