namespace HoneyWebPlatform.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Category;

    public class Flavour
    {
        public Flavour()
        {
            Propolises = new HashSet<Propolis>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Propolis> Propolises { get; set; }
    }
}