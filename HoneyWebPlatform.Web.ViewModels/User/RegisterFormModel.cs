namespace HoneyWebPlatform.Web.ViewModels.User
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    using Common;

    using static Common.EntityValidationConstants.User;

    public class RegisterFormModel
    {
        [Required(ErrorMessage = "Моля въведете вашия имейл.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        [Display(Name = "Имейл")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете вашата парола.")]
        [StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength,
            ErrorMessage = "Паролата трябва да е с дължина от {2} до {1} символа.")]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Потвърдете паролата")]
        [Compare("Password", ErrorMessage = "Паролата и потвърждението не съвпадат.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете вашето първо име.")]
        [StringLength(FirstNameMaxLength, MinimumLength = FirstNameMinLength,
            ErrorMessage = "Първото име трябва да е с дължина от {2} до {1} символа.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете вашата фамилия.")]
        [StringLength(LastNameMaxLength, MinimumLength = LastNameMinLength,
            ErrorMessage = "Фамилията трябва да е с дължина от {2} до {1} символа.")]
        public string LastName { get; set; } = null!;

        // New property for profile picture path
        [Display(Name = "Профилна снимка")]
        [MaxFileSize(ProfilePictureMaxSize, ErrorMessage = "Максималният размер на файла за профилната снимка е 2 мегабайта.")]
        public IFormFile? ProfilePicturePath { get; set; }
    }
}
