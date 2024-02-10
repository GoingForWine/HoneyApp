namespace HoneyWebPlatform.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;

    using Services.Data.Interfaces;

    using Web.ViewModels.User;

    using static Common.GeneralApplicationConstants;
    using static Common.NotificationMessagesConstants;


    public class UserController : BaseAdminController
    {
        private readonly IUserService userService;
        private readonly IMemoryCache memoryCache;
        private readonly ISubscribedEmailService subscribedEmailService;


        public UserController(IUserService userService, IMemoryCache memoryCache, ISubscribedEmailService subscribedEmailService)
        {
            this.userService = userService;
            this.memoryCache = memoryCache;
            this.subscribedEmailService = subscribedEmailService;
        }

        [Route("User/All")]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> All()
        {
            IEnumerable<UserViewModel> users =
                this.memoryCache.Get<IEnumerable<UserViewModel>>(UsersCacheKey);
            if (users == null)
            {
                users = await this.userService.AllAsync();

                MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan
                        .FromMinutes(UsersCacheDurationMinutes));

                this.memoryCache.Set(UsersCacheKey, users, cacheOptions);
            }

            return View(users);
        }


        [HttpGet]
        public async Task<IActionResult> ExportSubscribedEmails()
        {
            try
            {
                var subscribedEmails = await subscribedEmailService.GetSubscribedEmailsAsync();
                var emails = string.Join(", ", subscribedEmails.Select(email => email.Email));

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/exports", "SubscribedEmails.txt");
                System.IO.File.WriteAllText(filePath, emails);

                return File(System.IO.File.ReadAllBytes(filePath), "text/plain", "SubscribedEmails.txt");
            }
            catch (Exception ex)
            {
                // Handle exceptions
                TempData[ErrorMessage] = "Error exporting subscribed emails.";
                return RedirectToAction("All");
            }
        }
    }
}