namespace HoneyWebPlatform.Data.Models
{
    using System;

    public class CartItem
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }


        // Foreign key to link the cart item with a user's cart
        public Guid CartId { get; set; }
        // Navigation property to represent the user's cart
        public Cart Cart { get; set; }


        public Guid? HoneyId { get; set; }
        // Navigation property to represent the associated honey
        public Honey Honey { get; set; }

        public Guid? PropolisId { get; set; }
        // Navigation property to represent the associated propolis
        public Propolis Propolis { get; set; }
    }
}