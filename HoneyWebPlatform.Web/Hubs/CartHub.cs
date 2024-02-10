using HoneyWebPlatform.Services.Data.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace HoneyWebPlatform.Web.Hubs
{
    public class CartHub : Hub
    {
        private readonly ICartService cartService;

        public CartHub(ICartService cartService)
        {
            cartService = cartService;
        }

        // todo i think this method should require cartId, productId, quantity

        // todo this has to be referencing something else


        public async Task UpdateCart(string userId, decimal totalPrice)
        {
            // Send the updated total price to all clients in the group (userId)
            await Clients.Group(userId).SendAsync("UpdateCartTotal", totalPrice);
        }

        public async Task JoinGroup(string userId)
        {
            // Add the connection to a group based on userId
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        public async Task UpdateCartItemQuantity(Guid productId, int quantity)
        {
            // Update the quantity in the cart
            
            var success = await cartService.UpdateCartItemQuantityAsync(Context.UserIdentifier!, productId, quantity);

            if (success)
            {
                // Broadcast the updated quantity to all connected clients
                await Clients.All.SendAsync("CartItemQuantityUpdated", productId, quantity);
            }
        }
    }
}