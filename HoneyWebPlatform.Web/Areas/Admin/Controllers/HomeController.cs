namespace HoneyWebPlatform.Web.Areas.Admin.Controllers
{
    using HoneyWebPlatform.Services.Data.Interfaces;
    using HoneyWebPlatform.Services.Data.Models.Statistics;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseAdminController
    {

        private readonly IHoneyService honeyService;
        private readonly IPropolisService propolisService;

        public HomeController(IHoneyService honeyService, IPropolisService propolisService)
        {
            this.honeyService = honeyService;
            this.propolisService = propolisService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                StatisticsServiceModel serviceModel =
                    await honeyService.GetStatisticsAsync();

                return Ok(serviceModel);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}