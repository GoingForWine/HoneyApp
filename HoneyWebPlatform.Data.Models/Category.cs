namespace HoneyWebPlatform.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Category;

    public class Category
    {
        public Category()
        {
            Honeys = new HashSet<Honey>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Honey> Honeys { get; set; }
    }
}