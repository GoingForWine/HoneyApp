using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace HoneyWebPlatform.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using HoneyWebPlatform.Data;
    using HoneyWebPlatform.Data.Models;
    using Interfaces;
    using Web.ViewModels.User;

    public class OrderService : IOrderService
    {
        private readonly HoneyWebPlatformDbContext dbContext;

        public OrderService(HoneyWebPlatformDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Guid> CreateOrderAsync(CartViewModel cart)
        {
            if (cart == null || (!cart.Honeys.Any() && !cart.Propolises.Any()))
            {
                // Handle the case where the cart is empty or invalid
                return Guid.Empty;
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = cart.UserInformation.Id,
                PhoneNumber = cart.UserInformation.PhoneNumber,
                CreatedOn = DateTime.Now,
                Email = cart.UserInformation.Email,
                Address = cart.UserInformation.Address,
                TotalPrice = cart.Honeys.Sum(honey => honey.TotalPrice) + cart.Propolises.Sum(propolis => propolis.TotalPrice),
                Status = OrderStatus.Обработван
            };

            order.OrderItems = cart.Honeys.Concat(cart.Propolises).Select(product => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Quantity = product.Quantity,
                Price = product.Price,
                OrderId = order.Id
            }).ToList();

            dbContext.Orders.Add(order);
            //maybe clear cart
            await dbContext.SaveChangesAsync();

            return order.Id;
        }

        public async Task ClearUserCartAsync(string userId)
        {
            var cart = await dbContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                dbContext.CartItems.RemoveRange(cart.CartItems);
                await dbContext.SaveChangesAsync();
            }
        }


        public async Task<List<OrderViewModel>> GetAllOrdersAsync()
        {
            return await dbContext.Orders
                .Select(o => new OrderViewModel
                {
                    OrderId = o.Id,
                    OrderDate = o.CreatedOn,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status,
                    Address = o.Address,
                    // Add other properties as needed
                    OrderItems = o.OrderItems.Select(oi => new OrderItemViewModel
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        Price = oi.Price,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.Price * oi.Quantity
                        // Add other properties as needed
                    }).ToList(),
                    
                    //for admin only
                    Email = o.Email,
                    PhoneNumber = o.PhoneNumber,
                    UserId = o.UserId

                })
                .ToListAsync();
        }




        public async Task<Order> GetOrderAsync(Guid userId)
        {
            return await dbContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.UserId == userId);
        }

        public async Task<List<OrderViewModel>> GetUserOrdersAsync(string userId)
        {
            return await dbContext.Orders
                .Where(o => o.UserId.ToString() == userId)
                .Select(o => new OrderViewModel
                {
                    OrderId = o.Id,
                    OrderDate = o.CreatedOn,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status,
                    Address = o.Address,
                    // Add other properties as needed
                    OrderItems = o.OrderItems.Select(oi => new OrderItemViewModel
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        Price = oi.Price,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.Price * oi.Quantity
                        // Add other properties as needed
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null)
            {
                order.Status = newStatus;
                await dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }


        public async Task<bool> HasOrdersForUserAsync(string userId)
        {
            return await dbContext.Orders
                .AnyAsync(o => o.UserId.ToString() == userId);
        }


    }

}
