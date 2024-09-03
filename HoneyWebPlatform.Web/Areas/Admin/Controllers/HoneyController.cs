using HoneyWebPlatform.Common;

namespace HoneyWebPlatform.Web.Areas.Admin.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;

    using Services.Data.Interfaces;
    using ViewModels.Honey;

    using static NotificationMessagesConstants;

    public class HoneyController : BaseAdminController
    {
        private readonly IBeekeeperService beekeeperService;
        private readonly IHoneyService honeyService;

        public HoneyController(IBeekeeperService beekeeperService, IHoneyService honeyService)
        {
            this.beekeeperService = beekeeperService;
            this.honeyService = honeyService;
        }

        public async Task<IActionResult> Mine()
        {
            string? beekeeperId =
                await this.beekeeperService.GetBeekeeperIdByUserIdAsync(this.User.GetId()!);
            MyHoneysViewModel viewModel = new MyHoneysViewModel()
            {
                AddedHoneys = await this.honeyService.AllByBeekeeperIdAsync(beekeeperId!)
            };

            return View(viewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> Promote(string id)
        {
            // Check if the honey exists
            bool honeyExists = await honeyService.ExistsByIdAsync(id);
            if (!honeyExists)
            {
                TempData[ErrorMessage] = "Няма такъв мед!";
                return RedirectToAction("All", "Honey", new { area = "" });
            }

            // Check if the user is authorized to promote/demote the honey
            bool isUserAdmin = User.IsAdmin();
            if (!isUserAdmin)
            {
                TempData[ErrorMessage] = "Нямате право да променяте промоцията на меда!";
                return RedirectToAction("All", "Honey",new { area = "" });
            }

            try
            {
                // Toggle the promotion status of the honey
                await honeyService.TogglePromotionAsync(id);

                TempData[SuccessMessage] = "Промотирането/демотирането на меда беше успешно!";
            }
            catch (Exception)
            {
                TempData[ErrorMessage] = "Неуспех при промотирането!";
            }

            return RedirectToAction("All", "Honey", new { area = "" });
        }


    }
}