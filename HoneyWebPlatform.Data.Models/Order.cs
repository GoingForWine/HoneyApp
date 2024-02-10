namespace HoneyWebPlatform.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    using static Common.EntityValidationConstants.Beekeeper;

    public enum OrderStatus
    {
        Обработван,
        Приготвен,
        Изпратен,
        Приключен
    }

    public class Order
    {
        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }

        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        [Required]
        [MaxLength(PhoneNumberMaxLength)]
        public string PhoneNumber { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public string Email { get; set; } = null!; 

        public string Address { get; set; } = null!;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

    }
}
