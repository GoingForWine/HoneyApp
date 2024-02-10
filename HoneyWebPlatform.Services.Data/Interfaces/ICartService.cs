namespace HoneyWebPlatform.Services.Data.Interfaces
{
    using System.Threading.Tasks;

    using Web.ViewModels.User;

    public interface ICartService
    {
        Task<CartViewModel> GetCartAsync(string userId);

        Task<bool> AddToCartAsync(string userId, Guid honeyId, Guid propolisId, int quantity);

        Task<bool> RemoveFromCartAsync(string userId, Guid cartItemId);

        Task<bool> ClearCartAsync(string userId);

        Task<bool> UpdateCartItemQuantityAsync(string userId, Guid productId, int quantity);

    }
}