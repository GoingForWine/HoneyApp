namespace HoneyWebPlatform.Web.ViewModels.User
{
    using System.Collections.Generic;

    public class CartItemViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal TotalPrice => Quantity * Price;
    }

    public class CartViewModel
    {
        public List<CartItemViewModel> Honeys { get; set; } = new List<CartItemViewModel>();
        public List<CartItemViewModel> Propolises { get; set; } = new List<CartItemViewModel>();

        public decimal TotalPrice => Honeys.Sum(x => x.TotalPrice) + Propolises.Sum(x => x.TotalPrice);

        public UserViewModel UserInformation { get; set; } = null!;
    }
}