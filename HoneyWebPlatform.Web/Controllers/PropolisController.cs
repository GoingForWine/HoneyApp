namespace HoneyWebPlatform.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Infrastructure.Extensions;
    using Services.Data.Interfaces;
    using Services.Data.Models.Propolis;
    using ViewModels.Propolis;

    using static Common.GeneralApplicationConstants;
    using static Common.NotificationMessagesConstants;


    [Authorize]
    public class PropolisController : Controller
    {
        private readonly IFlavourService flavourService;
        private readonly IBeekeeperService beekeeperService;
        private readonly IPropolisService propolisService;

        public PropolisController(IFlavourService flavourService, IBeekeeperService beekeeperService,
            IPropolisService propolisService)
        {
            this.flavourService = flavourService;
            this.beekeeperService = beekeeperService;
            this.propolisService = propolisService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All([FromQuery] AllPropolisesQueryModel queryModel)
        {
            AllPropolisesFilteredAndPagedServiceModel serviceModel =
                await propolisService.AllAsync(queryModel);

            queryModel.Propolises = serviceModel.Propolises;
            queryModel.TotalPropolises = serviceModel.TotalPropolisesCount;
            queryModel.Flavours = await flavourService.AllFlavourNamesAsync();

            return View(queryModel);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            bool isBeekeeper =
                await beekeeperService.BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isBeekeeper)
            {
                TempData[ErrorMessage] = "You must become a Beekeeper in order to add new propolises!";

                return RedirectToAction("Become", "Beekeeper");
            }

            try
            {
                PropolisFormModel formModel = new PropolisFormModel()
                {
                    Flavours = await flavourService.AllFlavoursAsync()
                };

                return View(formModel);
            }
            catch (Exception)
            {
                return GeneralError();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(PropolisFormModel model)
        {
            bool isBeekeeper =
                await beekeeperService.BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isBeekeeper)
            {
                TempData[ErrorMessage] = "You must become a Beekeeper in order to add new Propolises!";

                return RedirectToAction("Become", "Beekeeper");
            }

            bool flavourExists =
                await flavourService.ExistsByIdAsync(model.FlavourId);
            if (!flavourExists)
            {
                // Adding model error to ModelState automatically makes ModelState Invalid
                ModelState.AddModelError(nameof(model.FlavourId), "Selected flavour does not exist!");
            }

            if (!ModelState.IsValid)
            {
                model.Flavours = await flavourService.AllFlavoursAsync();

                return View(model);
            }

            try
            {
                string? beekeeperId =
                    await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);

                string propolisId =
                    await propolisService.CreateAndReturnIdAsync(model, beekeeperId!);

                TempData[SuccessMessage] = "The Propolis was added successfully!";
                return RedirectToAction("Details", "Propolis", new { id = propolisId });
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred while trying to add your new Propolis! Please try again later or contact administrator!");
                model.Flavours = await flavourService.AllFlavoursAsync();

                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            bool propolisExists = await propolisService
                .ExistsByIdAsync(id);
            if (!propolisExists)
            {
                TempData[ErrorMessage] = "Propolis with the provided id does not exist!";

                return RedirectToAction("All", "Propolis");
            }

            try
            {
                PropolisDetailsViewModel viewModel = await propolisService
                    .GetDetailsByIdAsync(id);
                viewModel.Beekeeper.FullName =
                    await beekeeperService.BeekeeperFullnameByPropolisIdAsync(id);

                return View(viewModel);
            }
            catch (Exception)
            {
                return GeneralError();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            bool propolisExists = await propolisService
                .ExistsByIdAsync(id);
            if (!propolisExists)
            {
                TempData[ErrorMessage] = "Propolis with the provided id does not exist!";

                return RedirectToAction("All", "Propolis");

                //return this.NotFound(); -> If you want to return 404 page
            }

            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isUserBeekeeper && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must become a Beekeeper in order to edit Propolis info!";

                return RedirectToAction("Become", "Beekeeper");
            }

            string? beekeeperId =
                await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);
            bool isBeekeeperOwner = await propolisService
                .IsBeekeeperWithIdOwnerOfPropolisWithIdAsync(id, beekeeperId!);
            if (!isBeekeeperOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must be the Beekeeper owner of the Propolis you want to edit!";

                return RedirectToAction("Mine", "Propolis");
            }

            try
            {
                PropolisFormModel formModel = await propolisService
                    .GetPropolisForEditByIdAsync(id);
                formModel.Flavours = await flavourService.AllFlavoursAsync();

                return View(formModel);
            }
            catch (Exception)
            {
                return GeneralError();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, PropolisFormModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Flavours = await flavourService.AllFlavoursAsync();

                return View(model);
            }

            bool propolisExists = await propolisService
                .ExistsByIdAsync(id);
            if (!propolisExists)
            {
                TempData[ErrorMessage] = "Propolis with the provided id does not exist!";

                return RedirectToAction("All", "Propolis");
            }

            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isUserBeekeeper && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must become a Beekeeper in order to edit Propolis info!";

                return RedirectToAction("Become", "Beekeeper");
            }

            string? beekeeperId =
                await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);
            bool isBeekeeperOwner = await propolisService
                .IsBeekeeperWithIdOwnerOfPropolisWithIdAsync(id, beekeeperId!);
            if (!isBeekeeperOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must be the Beekeeper owner of the Propolis if you want to edit!";

                return RedirectToAction("Mine", "Propolis");
            }

            try
            {
                await propolisService.EditPropolisByIdAndFormModelAsync(id, model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty,
                    "Unexpected error occurred while trying to update the Propolis. Please try again later or contact administrator!");
                model.Flavours = await flavourService.AllFlavoursAsync();

                return View(model);
            }

            TempData[SuccessMessage] = "Propolis was edited successfully!";
            return RedirectToAction("Details", "Propolis", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            bool propolisExists = await propolisService
                .ExistsByIdAsync(id);
            if (!propolisExists)
            {
                TempData[ErrorMessage] = "Propolis with the provided id does not exist!";

                return RedirectToAction("All", "Propolis");
            }

            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isUserBeekeeper && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must become a Beekeeper in order to edit Propolis info!";

                return RedirectToAction("Become", "Beekeeper");
            }

            string? beekeeperId =
                await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);
            bool isBeekeeperOwner = await propolisService
                .IsBeekeeperWithIdOwnerOfPropolisWithIdAsync(id, beekeeperId!);
            if (!isBeekeeperOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must be the Beekeeper owner of the Propolis if you want to edit!";

                return RedirectToAction("Mine", "Propolis");
            }

            try
            {
                PropolisPreDeleteDetailsViewModel viewModel =
                    await propolisService.GetPropolisForDeleteByIdAsync(id);

                return View(viewModel);
            }
            catch (Exception)
            {
                return GeneralError();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, PropolisPreDeleteDetailsViewModel model)
        {
            bool propolisExists = await propolisService
                .ExistsByIdAsync(id);
            if (!propolisExists)
            {
                TempData[ErrorMessage] = "The Propolis with the provided id does not exist!";

                return RedirectToAction("All", "Propolis");
            }

            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isUserBeekeeper && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must become a Beekeeper in order to edit Propolis info!";

                return RedirectToAction("Become", "Beekeeper");
            }

            string? beekeeperId =
                await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);
            bool isBeekeeperOwner = await propolisService
                .IsBeekeeperWithIdOwnerOfPropolisWithIdAsync(id, beekeeperId!);
            if (!isBeekeeperOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must be the Beekeeper owner of the Propolis you want to edit!";

                return RedirectToAction("Mine", "Propolis");
            }

            try
            {
                await propolisService.DeletePropolisByIdAsync(id);

                TempData[WarningMessage] = "The Propolis was successfully deleted!";
                return RedirectToAction("Mine", "Propolis");
            }
            catch (Exception)
            {
                return GeneralError();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Mine()
        {
            if (this.User.IsInRole(AdminRoleName))
            {
                return this.RedirectToAction("Mine", "Propolis", new { Area = AdminAreaName });
            }

            List<PropolisAllViewModel> myPropolises =
                new List<PropolisAllViewModel>();

            string userId = User.GetId()!;
            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(userId);

            try
            {
                if (User.IsAdmin())
                {
                    string? beekeeperId =
                        await beekeeperService.GetBeekeeperIdByUserIdAsync(userId);

                    // Added houses as an Agent
                    myPropolises.AddRange(await propolisService.AllByBeekeeperIdAsync(beekeeperId!));

                    myPropolises = myPropolises
                        .DistinctBy(h => h.Id)
                        .ToList();
                }
                else if (isUserBeekeeper)
                {
                    string? beekeeperId =
                        await beekeeperService.GetBeekeeperIdByUserIdAsync(userId);

                    myPropolises.AddRange(await propolisService.AllByBeekeeperIdAsync(beekeeperId!));
                }
                else
                {
                    return GeneralError();
                }

                return View(myPropolises);
            }
            catch (Exception)
            {
                return GeneralError();
            }
        }

        private IActionResult GeneralError()
        {
            TempData[ErrorMessage] =
                "Unexpected error occurred! Please try again later or contact administrator";

            return RedirectToAction("Index", "Home");
        }
    }
}
