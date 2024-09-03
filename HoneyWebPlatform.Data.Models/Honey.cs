namespace HoneyWebPlatform.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Honey;

    public class Honey
    {
        public Honey()
        {
            Id = Guid.NewGuid();
            IsPromoted = false; //default value
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(OriginMaxLength)]
        public string Origin { get; set; } = null!;

        [Required]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required]
        [MaxLength(ImageUrlMaxLength)]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int NetWeight { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        public int YearMade { get; set; }

        public bool IsActive { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; } = null!;

        public Guid BeekeeperId { get; set; }

        public virtual Beekeeper Beekeeper { get; set; } = null!;

        //todo questionable if we should have a count of honeys available, maybe after 10 are bought, we run out

        public bool IsPromoted { get; set; }
    }
}
