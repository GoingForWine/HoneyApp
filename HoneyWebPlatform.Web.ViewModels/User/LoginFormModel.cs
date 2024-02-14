namespace HoneyWebPlatform.Web.ViewModels.User
{
    using System.ComponentModel.DataAnnotations;

    public class LoginFormModel
    {
        [Required(ErrorMessage = "Моля въведете вашия имейл.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете вашата парола.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Запомни ме")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}