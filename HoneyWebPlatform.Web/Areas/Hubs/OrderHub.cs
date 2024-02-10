namespace HoneyWebPlatform.Web.Areas.Hubs
{
    using Data.Models;
    using HoneyWebPlatform.Services.Data.Interfaces;
    using Microsoft.AspNetCore.SignalR;

    public class OrderHub : Hub
    {
        private readonly IOrderService orderService;

        public OrderHub(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public async Task UpdateOrderStatus(Guid orderId, OrderStatus newStatus)
        {
            try
            {
                // Update the order status in the database
                var success = await orderService.UpdateOrderStatusAsync(orderId, newStatus);

                if (success)
                {
                    // Notify clients about the status change
                    await Clients.All.SendAsync("OrderStatusUpdated", orderId, newStatus);
                }
                else
                {
                    // Log that the update failed
                    Console.WriteLine($"Failed to update order status for orderId: {orderId}");
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions
                Console.WriteLine($"Exception in UpdateOrderStatus: {ex.Message}");
                throw; // Rethrow the exception to see it in the console
            }
        }



        private OrderStatus ConvertToOrderStatus(int statusValue)
        {
            // Convert the integer value to OrderStatus enum
            // Adjust this logic based on your OrderStatus enum structure
            switch (statusValue)
            {
                case 0:
                    return OrderStatus.Обработван;
                case 1:
                    return OrderStatus.Приготвен;
                case 2:
                    return OrderStatus.Изпратен;
                case 3:
                    return OrderStatus.Приключен;
                default:
                    // You may want to handle unknown values differently
                    throw new ArgumentException("Unknown order status value", nameof(statusValue));
            }
        }
    }
}
