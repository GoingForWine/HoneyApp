namespace HoneyWebPlatform.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HoneyWebPlatform.Data.Models;
    using HoneyWebPlatform.Services.Data;
    using HoneyWebPlatform.Web.Areas.Hubs;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Services.Data.Interfaces;
    using Web.ViewModels.User;

    public class OrdersController : BaseAdminController
    {
        private readonly IOrderService orderService;
        private readonly IUserService userService;

        private readonly IHubContext<OrderHub> orderHubContext;


        public OrdersController(IOrderService orderService,
                                IUserService userService,
                                IHubContext<OrderHub> orderHubContext)
        {
            this.orderService = orderService;
            this.userService = userService;

            this.orderHubContext = orderHubContext;
        }

        [Route("Orders/All")]
        public async Task<IActionResult> All()
        {
            List<OrderViewModel> orders = await orderService.GetAllOrdersAsync();

            foreach (var order in orders)
            {
                // Load the user's full name asynchronously
                order.UserName = await userService.GetFullNameByIdAsync(order.UserId.ToString());
            }
            return View(orders);
        }


        [HttpPost]
        public async Task<IActionResult> ChangeOrderStatus(Guid orderId, OrderStatus newStatus)
        {
            // Update the order status in the database
            var success = await orderService.UpdateOrderStatusAsync(orderId, newStatus);

            if (success)
            {
                // Notify clients about the status change
                await orderHubContext.Clients.All.SendAsync("OrderStatusUpdated", orderId, newStatus);

                return Ok();
            }

            return BadRequest("Failed to update order status.");
        }

        // Add other actions as needed for order details, exports, etc.
    }
}