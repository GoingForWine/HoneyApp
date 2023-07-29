using System.Security.AccessControl;

namespace HoneyPlatform.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Beekeeper;

    public class Beekeeper
    {
        public Beekeeper()
        {
            Id = Guid.NewGuid();
            OwnedHoney = new HashSet<Honey>();
            OwnedPropolis = new HashSet<Propolis>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(PhoneNumberMaxLength)]
        public string PhoneNumber { get; set; } = null!;

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public virtual ICollection<Honey> OwnedHoney { get; set; }

        public virtual ICollection<Propolis> OwnedPropolis { get; set; }

    }
}
