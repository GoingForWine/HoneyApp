namespace HoneyWebPlatform.Services.Data.Interfaces
{
    using HoneyWebPlatform.Data.Models;
    using Web.ViewModels.User;

    public interface IOrderService
    {
        Task<Guid> CreateOrderAsync(CartViewModel cart);

        Task<Order> GetOrderAsync(Guid userId);

        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);

        Task<bool> HasOrdersForUserAsync(string userId);

        Task<List<OrderViewModel>> GetUserOrdersAsync(string userId);

        Task<List<OrderViewModel>> GetAllOrdersAsync();

    }
}