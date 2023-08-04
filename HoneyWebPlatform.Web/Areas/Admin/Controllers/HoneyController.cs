namespace HoneyWebPlatform.Web.Areas.Admin.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;

    using Services.Data.Interfaces;
    using ViewModels.Honey;

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

            return this.View(viewModel);
        }
    }
}