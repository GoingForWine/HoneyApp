namespace HoneyWebPlatform.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using HoneyWebPlatform.Data;
    using HoneyWebPlatform.Data.Models;
    using Interfaces;
    using Web.ViewModels.User;

    public class CartService : ICartService
    {
        private readonly HoneyWebPlatformDbContext dbContext;

        public CartService(HoneyWebPlatformDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<CartViewModel> GetCartAsync(string userId)
        {
            var cartViewModel = new CartViewModel();

            var cart = await dbContext.Carts
                .AsNoTracking()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Honey)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Propolis)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                foreach (var cartItem in cart.CartItems)
                {
                    var cartItemViewModel = new CartItemViewModel
                    {
                        ProductId = cartItem.Honey != null ? (Guid)cartItem.HoneyId : (Guid)cartItem.PropolisId,
                        ProductName = cartItem.Honey != null ? cartItem.Honey.Title : cartItem.Propolis.Title,
                        Price = cartItem.Honey != null ? cartItem.Honey.Price : cartItem.Propolis.Price,
                        ImageUrl = cartItem.Honey != null ? cartItem.Honey.ImageUrl : cartItem.Propolis.ImageUrl,
                        Quantity = cartItem.Quantity
                    };

                    if (cartItem.Honey != null)
                    {
                        cartViewModel.Honeys.Add(cartItemViewModel);
                    }
                    else if (cartItem.Propolis != null)
                    {
                        cartViewModel.Propolises.Add(cartItemViewModel);
                    }
                }
            }

            return cartViewModel;
        }

        public async Task<bool> UpdateCartItemQuantityAsync(string userId, Guid productId, int quantity)
        {
            var cartItem = await dbContext.CartItems
                .Include(ci => ci.Cart)
                .SingleOrDefaultAsync(c => c.Cart.UserId == userId &&
                                           (c.HoneyId == productId || c.PropolisId == productId));

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                await dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> AddToCartAsync(string userId, Guid honeyId, Guid propolisId, int quantity)
        {
            if (quantity <= 0)
            {
                return false;
            }

            var cart = await dbContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId
                };

                dbContext.Carts.Add(cart);

                await dbContext.SaveChangesAsync();
            }

            //You might want to add additional validation for the availability
            //of the product or any business rules related to adding items to the cart.

            if (honeyId != Guid.Empty)
            {
                var existingHoneyItem = cart.CartItems.FirstOrDefault(ci => ci.HoneyId == honeyId);

                if (existingHoneyItem != null)
                {
                    existingHoneyItem.Quantity += quantity;
                }
                else
                {
                    var newHoneyCartItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        HoneyId = honeyId,
                        Quantity = quantity,
                        CartId = cart.Id,
                        Cart = await dbContext.Carts.FindAsync(cart.Id),
                        Honey = await dbContext.Honeys.FindAsync(honeyId),
                        Propolis = null,
                        PropolisId = null
                    };

                    // Set EntityState to Added
                    dbContext.Entry(newHoneyCartItem).State = EntityState.Added;

                    cart.CartItems.Add(newHoneyCartItem);
                }
            }
            else if (propolisId != Guid.Empty)
            {
                var existingPropolisItem = cart.CartItems.FirstOrDefault(ci => ci.PropolisId == propolisId);

                if (existingPropolisItem != null)
                {
                    existingPropolisItem.Quantity += quantity;
                }
                else
                {
                    var newPropolisCartItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        PropolisId = propolisId,
                        Quantity = quantity,
                        CartId = cart.Id,
                        Cart = await dbContext.Carts.FindAsync(cart.Id),
                        Propolis = await dbContext.Propolises.FindAsync(propolisId),
                        Honey = null,
                        HoneyId = null
                    };

                    // Set EntityState to Added
                    dbContext.Entry(newPropolisCartItem).State = EntityState.Added;

                    cart.CartItems.Add(newPropolisCartItem);
                }
            }

            await dbContext.SaveChangesAsync();
            return true;
        }





        public async Task<bool> RemoveFromCartAsync(string userId, Guid cartItemId)
        {
            var cartItem = await dbContext.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Cart.UserId == userId &&
                                           (ci.HoneyId == cartItemId || ci.PropolisId == cartItemId));

            if (cartItem != null)
            {
                dbContext.CartItems.Remove(cartItem);
                await dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }


        public async Task<bool> ClearCartAsync(string userId)
        {
            var cart = await dbContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                dbContext.CartItems.RemoveRange(cart.CartItems);
                await dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}