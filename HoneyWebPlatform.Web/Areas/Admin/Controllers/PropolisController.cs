using HoneyWebPlatform.Common;

namespace HoneyWebPlatform.Web.Areas.Admin.Controllers
{
    using HoneyWebPlatform.Services.Data;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;

    using Services.Data.Interfaces;
    using ViewModels.Propolis;
    
    using static NotificationMessagesConstants;

    public class PropolisController : BaseAdminController
    {
        private readonly IBeekeeperService beekeeperService;
        private readonly IPropolisService propolisService;

        public PropolisController(IBeekeeperService beekeeperService, IPropolisService propolisService)
        {
            this.beekeeperService = beekeeperService;
            this.propolisService = propolisService;
        }

        public async Task<IActionResult> Mine()
        {
            string? beekeeperId =
                await this.beekeeperService.GetBeekeeperIdByUserIdAsync(this.User.GetId()!);
            MyPropolisesViewModel viewModel = new MyPropolisesViewModel()
            {
                AddedPropolises = await this.propolisService.AllByBeekeeperIdAsync(beekeeperId!)
            };

            return this.View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Promote(string id)
        {
            // Check if the honey exists
            bool propolisExists = await propolisService.ExistsByIdAsync(id);
            if (!propolisExists)
            {
                TempData[ErrorMessage] = "Няма такъв прополис!";
                return RedirectToAction("All", "Propolis", new { area = "" });
            }

            // Check if the user is authorized to promote/demote the honey
            bool isUserAdmin = User.IsAdmin();
            if (!isUserAdmin)
            {
                TempData[ErrorMessage] = "Нямате право да променяте промоцията на прополиса!";
                return RedirectToAction("All", "Propolis", new { area = "" });
            }

            try
            {
                // Toggle the promotion status of the honey
                await propolisService.TogglePromotionAsync(id);

                TempData[SuccessMessage] = "Промотирането/демотирането на меда беше успешно!";
            }
            catch (Exception)
            {
                TempData[ErrorMessage] = "Неуспех при промотирането!";
            }

            return RedirectToAction("All", "Propolis", new { area = "" });
        }

    }
}