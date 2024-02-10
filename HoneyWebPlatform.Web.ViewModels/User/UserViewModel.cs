namespace HoneyWebPlatform.Web.ViewModels.User
{
    using System.ComponentModel.DataAnnotations;

    public class UserViewModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Моля, въведете телефонния си номер.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Телефонният номер трябва да е от 10 цифри.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Моля, въведете адрес за доставка.")]
        public string Address { get; set; } = null!;

        public bool IsSubscribed { get; set; } = false;
    }
}
