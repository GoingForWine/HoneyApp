namespace HoneyWebPlatform.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Services.Data.Interfaces;

    using ViewModels.Home;
    using static Common.GeneralApplicationConstants;

    public class HomeController : Controller
    {
        private readonly IHoneyService honeyService;
        private readonly IPropolisService propolisService;

        public HomeController(IHoneyService honeyService, IPropolisService propolisService)
        {
            this.honeyService = honeyService;
            this.propolisService = propolisService;
        }

        public async Task<IActionResult> Index()
        {
            if (this.User.IsInRole(AdminRoleName))
            {
                return this.RedirectToAction("Index", "Home", new { Area = AdminAreaName });
            }

            IEnumerable<HoneyIndexViewModel> honeyIndexViewModel =
                await honeyService.LastThreeHoneysAsync();

            IEnumerable<PropolisIndexViewModel> propolisIndexViewModel =
               await propolisService.LastThreePropolisеsAsync();


            var viewModel = new IndexViewModel
            {
                Honeys = honeyIndexViewModel,
                Propolises = propolisIndexViewModel
            };


            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode)
        {
            if (statusCode == 400 || statusCode == 404)
            {
                return View("Error404");
            }

            if (statusCode == 401)
            {
                return View("Error401");
            }

            return View();
        }
    }
}