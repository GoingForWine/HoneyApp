namespace HoneyWebPlatform.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Infrastructure.Extensions;
    using Microsoft.Extensions.Caching.Memory;
    using Services.Data.Interfaces;
    using Services.Data.Models.Honey;
    using ViewModels.Honey;

    using static Common.GeneralApplicationConstants;
    using static Common.NotificationMessagesConstants;
    using HoneyWebPlatform.Services.Data;

    [Authorize]
    public class HoneyController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IBeekeeperService beekeeperService;
        private readonly IHoneyService honeyService;
        private readonly IUserService userService;

        private readonly IMemoryCache memoryCache;

        public HoneyController(ICategoryService categoryService, IBeekeeperService beekeeperService,
            IHoneyService honeyService, IUserService userService, IMemoryCache memoryCache)
        {
            this.categoryService = categoryService;
            this.beekeeperService = beekeeperService;
            this.honeyService = honeyService;
            this.userService = userService;

            this.memoryCache = memoryCache;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All([FromQuery] AllHoneysQueryModel queryModel)
        {
            AllHoneysFilteredAndPagedServiceModel serviceModel =
                await honeyService.AllAsync(queryModel);

            queryModel.Honeys = serviceModel.Honeys;
            queryModel.TotalHoneys = serviceModel.TotalHoneysCount;
            queryModel.Categories = await categoryService.AllCategoryNamesAsync();

            return View(queryModel);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            bool isBeekeeper =
                await beekeeperService.BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isBeekeeper)
            {
                TempData[ErrorMessage] = "You must become a Beekeeper in order to add new honeys!";

                return RedirectToAction("Become", "Beekeeper");
            }

            try
            {
                HoneyFormModel formModel = new HoneyFormModel()
                {
                    Categories = await categoryService.AllCategoriesAsync()
                };

                return View(formModel);
            }
            catch (Exception)
            {
                return GeneralError();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(HoneyFormModel model)
        {
            bool isBeekeeper =
                await beekeeperService.BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isBeekeeper)
            {
                TempData[ErrorMessage] = "You must become a Beekeeper in order to add new honeys!";

                return RedirectToAction("Become", "Beekeeper");
            }

            bool categoryExists =
                await categoryService.ExistsByIdAsync(model.CategoryId);
            if (!categoryExists)
            {
                // Adding model error to ModelState automatically makes ModelState Invalid
                ModelState.AddModelError(nameof(model.CategoryId), "Selected category does not exist!");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await categoryService.AllCategoriesAsync();

                return View(model);
            }

            try
            {
                string? beekeeperId =
                    await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);

                string honeyId =
                    await honeyService.CreateAndReturnIdAsync(model, beekeeperId!);

                TempData[SuccessMessage] = "The Honey was added successfully!";
                return RedirectToAction("Details", "Honey", new { id = honeyId });
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred while trying to add your new honey! Please try again later or contact administrator!");
                model.Categories = await categoryService.AllCategoriesAsync();

                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            bool honeyExists = await honeyService
                .ExistsByIdAsync(id);
            if (!honeyExists)
            {
                TempData[ErrorMessage] = "Honey with the provided id does not exist!";

                return RedirectToAction("All", "Honey");
            }

            try
            {
                HoneyDetailsViewModel viewModel = await honeyService
                    .GetDetailsByIdAsync(id);
                viewModel.Beekeeper.FullName =
                    await beekeeperService.BeekeeperFullnameByHoneyIdAsync(id);

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
            bool honeyExists = await honeyService
                .ExistsByIdAsync(id);
            if (!honeyExists)
            {
                TempData[ErrorMessage] = "Honey with the provided id does not exist!";

                return RedirectToAction("All", "Honey");

                //return this.NotFound(); -> If you want to return 404 page
            }

            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isUserBeekeeper && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must become a Beekeeper in order to edit honey info!";

                return RedirectToAction("Become", "Beekeeper");
            }

            string? beekeeperId =
                await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);
            bool isBeekeeperOwner = await honeyService
                .IsBeekeeperWithIdOwnerOfHoneyWithIdAsync(id, beekeeperId!);
            if (!isBeekeeperOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must be the Beekeeper owner of the honey you want to edit!";

                return RedirectToAction("Mine", "Honey");
            }

            try
            {
                HoneyFormModel formModel = await honeyService
                    .GetHoneyForEditByIdAsync(id);
                formModel.Categories = await categoryService.AllCategoriesAsync();

                return View(formModel);
            }
            catch (Exception)
            {
                return GeneralError();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, HoneyFormModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await categoryService.AllCategoriesAsync();

                return View(model);
            }

            bool honeyExists = await honeyService
                .ExistsByIdAsync(id);
            if (!honeyExists)
            {
                TempData[ErrorMessage] = "Honey with the provided id does not exist!";

                return RedirectToAction("All", "Honey");
            }

            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isUserBeekeeper && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must become a Beekeeper in order to edit honey info!";

                return RedirectToAction("Become", "Beekeeper");
            }

            string? beekeeperId =
                await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);
            bool isBeekeeperOwner = await honeyService
                .IsBeekeeperWithIdOwnerOfHoneyWithIdAsync(id, beekeeperId!);
            if (!isBeekeeperOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must be the Beekeeper owner of the honey if you want to edit!";

                return RedirectToAction("Mine", "Honey");
            }

            try
            {
                await honeyService.EditHoneyByIdAndFormModelAsync(id, model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty,
                    "Unexpected error occurred while trying to update the honey. Please try again later or contact administrator!");
                model.Categories = await categoryService.AllCategoriesAsync();

                return View(model);
            }

            TempData[SuccessMessage] = "Honey was edited successfully!";
            return RedirectToAction("Details", "Honey", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            bool honeyExists = await honeyService
                .ExistsByIdAsync(id);
            if (!honeyExists)
            {
                TempData[ErrorMessage] = "Honey with the provided id does not exist!";

                return RedirectToAction("All", "Honey");
            }

            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isUserBeekeeper && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must become a Beekeeper in order to edit honey info!";

                return RedirectToAction("Become", "Beekeeper");
            }

            string? beekeeperId =
                await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);
            bool isBeekeeperOwner = await honeyService
                .IsBeekeeperWithIdOwnerOfHoneyWithIdAsync(id, beekeeperId!);
            if (!isBeekeeperOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must be the Beekeeper owner of the honey if you want to edit!";

                return RedirectToAction("Mine", "Honey");
            }

            try
            {
                HoneyPreDeleteDetailsViewModel viewModel =
                    await honeyService.GetHoneyForDeleteByIdAsync(id);

                return View(viewModel);
            }
            catch (Exception)
            {
                return GeneralError();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, HoneyPreDeleteDetailsViewModel model)
        {
            bool honeyExists = await honeyService
                .ExistsByIdAsync(id);
            if (!honeyExists)
            {
                TempData[ErrorMessage] = "The Honey with the provided id does not exist!";

                return RedirectToAction("All", "Honey");
            }

            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isUserBeekeeper && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must become a Beekeeper in order to edit honey info!";

                return RedirectToAction("Become", "Beekeeper");
            }

            string? beekeeperId =
                await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);
            bool isBeekeeperOwner = await honeyService
                .IsBeekeeperWithIdOwnerOfHoneyWithIdAsync(id, beekeeperId!);
            if (!isBeekeeperOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must be the Beekeeper owner of the honey you want to edit!";

                return RedirectToAction("Mine", "Honey");
            }

            try
            {
                await honeyService.DeleteHoneyByIdAsync(id);

                TempData[WarningMessage] = "The honey was successfully deleted!";
                return RedirectToAction("Mine", "Honey");
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
                return this.RedirectToAction("Mine", "Honey", new { Area = AdminAreaName });
            }

            List<HoneyAllViewModel> myHoneys =
                new List<HoneyAllViewModel>();

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
                    myHoneys.AddRange(await honeyService.AllByBeekeeperIdAsync(beekeeperId!));

                    myHoneys = myHoneys
                        .DistinctBy(h => h.Id)
                        .ToList();
                }
                else if (isUserBeekeeper)
                {
                    string? beekeeperId =
                        await beekeeperService.GetBeekeeperIdByUserIdAsync(userId);

                    myHoneys.AddRange(await honeyService.AllByBeekeeperIdAsync(beekeeperId!));
                }
                else
                {
                    return GeneralError();
                }

                return View(myHoneys);
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
