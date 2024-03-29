﻿namespace HoneyWebPlatform.Data.Models
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
            OwnedBeePollen = new HashSet<BeePollen>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(PhoneNumberMaxLength)]
        public string PhoneNumber { get; set; } = null!;
        
        [MaxLength(255)]
        public string HiveFarmPicturePaths { get; set; }

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public virtual ICollection<Honey> OwnedHoney { get; set; }

        public virtual ICollection<Propolis> OwnedPropolis { get; set; }

        public virtual ICollection<BeePollen> OwnedBeePollen { get; set; }
    }
}
