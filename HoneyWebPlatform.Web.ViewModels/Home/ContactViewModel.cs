namespace HoneyWebPlatform.Web.ViewModels.Home
{
    using System.ComponentModel.DataAnnotations;

    public class ContactViewModel
    {
        [Required(ErrorMessage = "Името е задължително.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Името трябва да бъде между 3 и 50 символа.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Имейлът е задължителен.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Невалиден формат на имейл. Използвайте само латински символи.")]
        [StringLength(100, ErrorMessage = "Имейлът трябва да бъде максимум {1} символа.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Темата е задължителна.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Темата трябва да бъде между 3 и 100 символа.")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Съобщението е задължително.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Съобщението трябва да бъде между 10 и 1000 символа.")]
        public string Message { get; set; } = null!;

        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Телефонният номер трябва да съдържа 10 цифри.")]
        public string? Number { get; set; }
    }

}
