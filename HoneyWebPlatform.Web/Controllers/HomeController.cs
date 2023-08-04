namespace HoneyWebPlatform.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Services.Data.Interfaces;

    using static Common.GeneralApplicationConstants;
    using ViewModels.Home;

    public class HomeController : Controller
    {
        private readonly IHoneyService honeyService;

        public HomeController(IHoneyService honeyService)
        {
            this.honeyService = honeyService;
        }

        public async Task<IActionResult> Index()
        {
            if (this.User.IsInRole(AdminRoleName))
            {
                return this.RedirectToAction("Index", "Home", new { Area = AdminAreaName });
            }

            IEnumerable<IndexViewModel> viewModel =
                await honeyService.LastThreeHoneysAsync();

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