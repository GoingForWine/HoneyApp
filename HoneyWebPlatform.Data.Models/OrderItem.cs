using System.ComponentModel.DataAnnotations;
using HoneyWebPlatform.Common;

namespace HoneyWebPlatform.Data.Models
{
    using System;

    using static EntityValidationConstants.Honey;
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }


        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [MaxLength(TitleMaxLength), MinLength(TitleMinLength)]
        public string ProductName { get; set; } = null!;


        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Order Order { get; set; }
    }
}
