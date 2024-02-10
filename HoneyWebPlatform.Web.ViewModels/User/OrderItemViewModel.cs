namespace HoneyWebPlatform.Web.ViewModels.User
{
    public class OrderItemViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        //public string ImageUrl { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

}
