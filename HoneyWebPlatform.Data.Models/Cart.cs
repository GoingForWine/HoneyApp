namespace HoneyWebPlatform.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class Cart
    {
        public Guid Id { get; set; }

        // Foreign key to link the cart with a user
        public string UserId { get; set; }

        // Navigation property to represent the user's cart items
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}