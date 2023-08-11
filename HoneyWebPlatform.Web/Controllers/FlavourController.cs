namespace HoneyWebPlatform.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Infrastructure.Extensions;
    using Services.Data.Interfaces;
    using ViewModels.Flavour;

    [Authorize]
    public class FlavourController : Controller
    {
        private readonly IFlavourService flavourService;

        public FlavourController(IFlavourService flavourService)
        {
            this.flavourService = flavourService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            IEnumerable<AllFlavoursViewModel> viewModel =
                await flavourService.AllFlavoursForListAsync();

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, string information)
        {
            bool flavourExists = await flavourService.ExistsByIdAsync(id);
            if (!flavourExists)
            {
                return NotFound();
            }

            FlavourDetailsViewModel viewModel =
                await flavourService.GetDetailsByIdAsync(id);
            if (viewModel.GetUrlInformation() != information)
            {
                return NotFound();
            }

            return View(viewModel);
        }
    }
}
