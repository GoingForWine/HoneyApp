namespace HoneyWebPlatform.Web.Areas.Admin.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;

    using Services.Data.Interfaces;
    using ViewModels.Propolis;

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
    }
}