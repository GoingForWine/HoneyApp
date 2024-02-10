namespace HoneyWebPlatform.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    public class SubscribedEmail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}