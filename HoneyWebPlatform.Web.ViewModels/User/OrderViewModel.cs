namespace HoneyWebPlatform.Web.ViewModels.User
{
    using Data.Models;
    using System;

    public class OrderViewModel
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }

        public string Address { get; set; } = null!;
        public List<OrderItemViewModel> OrderItems { get; set; }


        //for admin only
        public string PhoneNumber { get; set; } = null!;

        public Guid UserId { get; set; }

        public string Email { get; set; } = null!;

        public string UserName { get; set; }

    }

}
