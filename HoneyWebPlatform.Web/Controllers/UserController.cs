namespace HoneyWebPlatform.Web.Controllers
{
    using Griesoft.AspNetCore.ReCaptcha;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.AspNetCore.SignalR;

    using Hubs;
    using Data.Models;
    using ViewModels.User;
    using HoneyWebPlatform.Services.Data.Interfaces;
    using static Common.GeneralApplicationConstants;
    using static Common.NotificationMessagesConstants;
    using Microsoft.AspNetCore.Hosting;

    public class UserController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        private readonly IMemoryCache memoryCache;

        private readonly ISubscribedEmailService subscribedEmailService;
        private readonly ICartService cartService;
        private readonly IOrderService orderService;

        private readonly IHubContext<CartHub> hubContext;

        public UserController(SignInManager<ApplicationUser> signInManager,
                              UserManager<ApplicationUser> userManager,
                              IMemoryCache memoryCache,
                              ISubscribedEmailService subscribedEmailService,
                              ICartService cartService,
                              IOrderService orderService,
                              IHubContext<CartHub> hubContext,
                              IWebHostEnvironment webHostEnvironment)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.webHostEnvironment = webHostEnvironment;

            this.memoryCache = memoryCache;

            this.subscribedEmailService = subscribedEmailService;
            this.cartService = cartService;
            this.orderService = orderService;

            this.hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Register(string? returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            RegisterFormModel model = new RegisterFormModel()
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateRecaptcha(Action = nameof(Register),
            ValidationFailedAction = ValidationFailedAction.ContinueRequest)]
        public async Task<IActionResult> Register(RegisterFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName
                // Add more properties as needed
            };

            await userManager.SetEmailAsync(user, model.Email);
            await userManager.SetUserNameAsync(user, model.Email);

            if (model.ProfilePicturePath != null && model.ProfilePicturePath.Length > 0)
            {
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "UsersProfilePictures");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePicturePath.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicturePath.CopyToAsync(fileStream);
                }
                user.ProfilePicturePath = "/uploads/UsersProfilePictures/" + uniqueFileName;
            }


            IdentityResult result =
                await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            await signInManager.SignInAsync(user, false);
            this.memoryCache.Remove(UsersCacheKey);

            return Redirect(model.ReturnUrl ?? "/Home/Index");
        }

        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            LoginFormModel model = new LoginFormModel()
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result =
                await signInManager
                    .PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (!result.Succeeded)
            {
                TempData[ErrorMessage] =
                    "There was an error while logging you in! Please try again later or contact an administrator.";

                return View(model);
            }

            return Redirect(model.ReturnUrl ?? "/Home/Index");
        }


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }










        [HttpPost]
        public async Task<IActionResult> SubscribeNewsletter(string email)
        {
            try
            {
                if (IsValidEmail(email))
                {
                    // Check if the email is already a registered user
                    var existingUser = await userManager.FindByEmailAsync(email);
                    if (existingUser != null)
                    {
                        existingUser.IsSubscribed = true;
                        await userManager.UpdateAsync(existingUser);
                    }

                    // Add the email to the SubscribedEmails table
                    await subscribedEmailService.AddSubscribedEmailAsync(email);

                    TempData[SuccessMessage] = "Успешно се записахте за е-вестника!";
                    return Redirect(HttpContext.Request.Headers["Referer"].ToString());

                }
                else
                {
                    TempData[ErrorMessage] = "Неправилен формат.";
                    return Redirect(HttpContext.Request.Headers["Referer"].ToString());
                }
            }
            catch (Exception)
            {
                // Handle exceptions
                TempData[ErrorMessage] = "Грешка при записването за е-вестника.";
                return Redirect(HttpContext.Request.Headers["Referer"].ToString());
            }
        }


        [HttpPost]
        public async Task<IActionResult> UnsubscribeNewsletter(string email)
        {
            try
            {
                if (IsValidEmail(email))
                {
                    // Check if the email is already a registered user
                    var existingUser = await userManager.FindByEmailAsync(email);
                    if (existingUser != null)
                    {
                        existingUser.IsSubscribed = false;
                        await userManager.UpdateAsync(existingUser);
                    }

                    // Remove the email from the SubscribedEmails table
                    await subscribedEmailService.RemoveSubscribedEmailAsync(email);

                    TempData[SuccessMessage] = "Успешно се отписахте от е-вестника!";
                    return Redirect(HttpContext.Request.Headers["Referer"].ToString());
                }
                else
                {
                    TempData[ErrorMessage] = "Неправилен формат.";
                    return Redirect(HttpContext.Request.Headers["Referer"].ToString());
                }
            }
            catch (Exception)
            {
                // Handle exceptions
                TempData[ErrorMessage] = "Грешка при отписването от е-вестника.";
                return Redirect(HttpContext.Request.Headers["Referer"].ToString());
            }
        }



        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            // Get the current user's cart
            string userId = userManager.GetUserId(User);
            CartViewModel cart = await cartService.GetCartAsync(userId);

            if (cart == null)
            {
                // Log or debug the issue
                TempData[ErrorMessage] = "Невалидна поръчка. Количката е празна или невалидна.";
                return RedirectToAction("Cart");
            }

            // Get user information
            var user = await userManager.FindByIdAsync(userId);

            // Pass user information to the view
            var userInformation = new UserViewModel()
            {
                FullName = user.FirstName + " " + user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email
            };

            // Combine the cart and user information in a single view model
            var cartViewModel = new CartViewModel
            {
                Honeys = cart.Honeys,
                Propolises = cart.Propolises,
                UserInformation = userInformation
            };

            return View(cartViewModel);
        }

        // Add actions for adding/removing items to/from the cart using the CartService
        // ...

        // Example action for adding a product to the cart
        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid honeyId, Guid propolisId, int quantity)
        {
            string userId = userManager.GetUserId(User);
            bool result = await cartService.AddToCartAsync(userId, honeyId, propolisId, quantity);

            if (result)
            {
                TempData[SuccessMessage] = "Продуктът беше успешно добавен в количката!";
            }
            else
            {
                TempData[ErrorMessage] = "Неуспешно добавяне на продукта в количката.";
            }

            return RedirectToAction("Cart");
        }

        // Example action for removing a product from the cart
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(Guid cartItemId)
        {
            string userId = userManager.GetUserId(User);
            bool result = await cartService.RemoveFromCartAsync(userId, cartItemId);

            if (result)
            {
                TempData[SuccessMessage] = "Продуктът беше успешно изкаран от количката!!";
            }
            else
            {
                TempData[ErrorMessage] = "Неуспешно изкарване на продукта от количката.";
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            string userId = userManager.GetUserId(User);
            bool result = await cartService.ClearCartAsync(userId);

            if (result)
            {
                TempData[SuccessMessage] = "Количката беше успешно изчистена!";
            }
            else
            {
                TempData[ErrorMessage] = "Неуспешно изчистване на количката.";
            }

            return RedirectToAction("Cart");
        }


        [HttpPost]
        public async Task<IActionResult> PlaceOrder(UserViewModel model)
        {
            string userId = userManager.GetUserId(User);

            CartViewModel cart = await cartService.GetCartAsync(userId);

            if (cart == null || (!cart.Honeys.Any() && !cart.Propolises.Any()))
            {
                // Log or debug the issue
                TempData[ErrorMessage] = "Невалидна поръчка. Количката е празна или съдържа невалидни продукти.";
                return RedirectToAction("Cart");
            }

            cart.UserInformation = model;
            cart.UserInformation.Id = Guid.Parse(userId);

            Guid orderId = await orderService.CreateOrderAsync(cart);

            if (orderId == Guid.Empty)
            {
                TempData[ErrorMessage] = "Неуспешно създаване на поръчка. Моля, опитайте отново.";
            }
            else
            {
                TempData[SuccessMessage] = "Успешно създадена поръчка с номер " + orderId;

                await cartService.ClearCartAsync(userId);
            }

            return RedirectToAction("Orders");

        }


        public async Task<IActionResult> Orders()
        {
            string userId = userManager.GetUserId(User);

            // Retrieve the user's orders from the database using the OrderService
            List<OrderViewModel> orders = await orderService.GetUserOrdersAsync(userId);

            return View(orders);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateCartItem(Guid productId, int quantity)
        {
            string userId = userManager.GetUserId(User);

            // Update the quantity in the cart
            var success = await cartService.UpdateCartItemQuantityAsync(userId, productId, quantity);

            if (success)
            {
                // Notify clients using SignalR
                await hubContext.Clients.All.SendAsync("CartItemQuantityUpdated", productId, quantity);

                // Return the updated total price
                var updatedCart = await cartService.GetCartAsync(userId);
                return Json(new { success = true, totalPrice = updatedCart.TotalPrice });
            }

            return Json(new { success = false });
        }





        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}
