namespace HoneyWebPlatform.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Hosting;

    using Infrastructure.Extensions;
    using Services.Data.Interfaces;
    using Services.Data.Models.Honey;
    using ViewModels.Honey;

    using static Common.GeneralApplicationConstants;
    using static Common.NotificationMessagesConstants;

    [Authorize]
    public class HoneyController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IBeekeeperService beekeeperService;
        private readonly IHoneyService honeyService;

        private readonly IWebHostEnvironment webHostEnvironment;


        public HoneyController(ICategoryService categoryService, IBeekeeperService beekeeperService,
            IHoneyService honeyService, IWebHostEnvironment webHostEnvironment)
        {
            this.categoryService = categoryService;
            this.beekeeperService = beekeeperService;
            this.honeyService = honeyService;
            this.webHostEnvironment = webHostEnvironment;
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
                TempData[ErrorMessage] = "Трябва да станеш пчелар за да създадеш мед!";

                return RedirectToAction("Become", "Beekeeper");
            }

            bool categoryExists =
                await categoryService.ExistsByIdAsync(model.CategoryId);
            if (!categoryExists)
            {
                // Adding model error to ModelState automatically makes ModelState Invalid
                ModelState.AddModelError(nameof(model.CategoryId), "Тази категория не съществува!");
            }

            //if (!ModelState.IsValid)
            //{
            //    model.Categories = await categoryService.AllCategoriesAsync();

            //    return View(model);
            //}

            try
            {
                string? beekeeperId =
                    await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);

                // Picture saving logic
                if (model.HoneyPicture != null && model.HoneyPicture.Length > 0)
                {
                    var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "HoneyPictures");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.HoneyPicture.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.HoneyPicture.CopyToAsync(fileStream);
                    }

                    model.HoneyPicturePath = "/uploads/HoneyPictures/" + uniqueFileName;
                }

                string honeyId =
                    await honeyService.CreateAndReturnIdAsync(model, beekeeperId!);

                TempData[SuccessMessage] = "Успешно добавихте мед!";

                return RedirectToAction("Details", "Honey", new { id = honeyId });
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Неочакван проблем стана докато опитвахме да добавим Вашия мед! Моля опитайте пак след малко или се свържете с администратор!");
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
                TempData[ErrorMessage] = "Няма такъв мед!";

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
                TempData[ErrorMessage] = "Мед с такова ИД няма!";

                return RedirectToAction("All", "Honey");

                //return this.NotFound(); -> If you want to return 404 page
            }

            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(User.GetId()!);

            if (!isUserBeekeeper && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Нямате право да променяте този мед!";

                return RedirectToAction("Become", "Beekeeper");
            }

            string? beekeeperId =
                await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);
            bool isBeekeeperOwner = await honeyService
                .IsBeekeeperWithIdOwnerOfHoneyWithIdAsync(id, beekeeperId!);

            if (!isBeekeeperOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Този мед трябва да е ваш, за да го променяте!";

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
            //if (!ModelState.IsValid)
            //{
            //    model.Categories = await categoryService.AllCategoriesAsync();

            //    return View(model);
            //}

            bool honeyExists = await honeyService
                .ExistsByIdAsync(id);

            if (!honeyExists)
            {
                TempData[ErrorMessage] = "Няма такъв мед!";

                return RedirectToAction("All", "Honey");
            }

            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(User.GetId()!);

            if (!isUserBeekeeper && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Нямате право да променяте меда!";

                return RedirectToAction("Become", "Beekeeper");
            }

            string? beekeeperId =
                await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);
            bool isBeekeeperOwner = await honeyService
                .IsBeekeeperWithIdOwnerOfHoneyWithIdAsync(id, beekeeperId!);
            if (!isBeekeeperOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Трябва да сте собственик, за да променяте меда!";

                return RedirectToAction("Mine", "Honey");
            }

            try
            {
                // Retrieve the old honey data
                var oldHoney = await honeyService.GetHoneyForEditByIdAsync(id);
                string oldPicturePath = oldHoney.HoneyPicturePath;

                // Picture saving logic for the new picture
                if (model.HoneyPicture != null && model.HoneyPicture.Length > 0)
                {
                    var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "HoneyPictures");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.HoneyPicture.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.HoneyPicture.CopyToAsync(fileStream);
                    }

                    model.HoneyPicturePath = "/uploads/HoneyPictures/" + uniqueFileName;
                }

                // Delete the old picture file
                if (!string.IsNullOrEmpty(oldPicturePath))
                {
                    var oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, oldPicturePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                await honeyService.EditHoneyByIdAndFormModelAsync(id, model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty,
                    "Неочакван проблем стана докато опитвахме да добавим Вашия мед! Моля опитайте пак след малко или се свържете с администратор!");
                model.Categories = await categoryService.AllCategoriesAsync();

                return View(model);
            }

            TempData[SuccessMessage] = "Медът беше успешно променен!";
            return RedirectToAction("Details", "Honey", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            bool honeyExists = await honeyService
                .ExistsByIdAsync(id);
            if (!honeyExists)
            {
                TempData[ErrorMessage] = "Няма такъв мед!";

                return RedirectToAction("All", "Honey");
            }

            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isUserBeekeeper && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Нямате право да променяте меда!";

                return RedirectToAction("Become", "Beekeeper");
            }

            string? beekeeperId =
                await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);
            bool isBeekeeperOwner = await honeyService
                .IsBeekeeperWithIdOwnerOfHoneyWithIdAsync(id, beekeeperId!);
            if (!isBeekeeperOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Трябва да сте собственик, за да променяте меда!";

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
                TempData[ErrorMessage] = "Няма такъв мед!";

                return RedirectToAction("All", "Honey");
            }

            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isUserBeekeeper && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Нямате право да променяте меда";

                return RedirectToAction("Become", "Beekeeper");
            }

            string? beekeeperId =
                await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);
            bool isBeekeeperOwner = await honeyService
                .IsBeekeeperWithIdOwnerOfHoneyWithIdAsync(id, beekeeperId!);
            if (!isBeekeeperOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Трябва да сте собственик, за да променяте меда!";

                return RedirectToAction("Mine", "Honey");
            }

            try
            {
                // Retrieve the old honey data to obtain the old picture path
                var oldHoney = await honeyService.GetHoneyForEditByIdAsync(id);
                string oldPicturePath = oldHoney.HoneyPicturePath;

                // Delete the old picture file from the server if it exists
                if (!string.IsNullOrEmpty(oldPicturePath))
                {
                    var oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, oldPicturePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }


                await honeyService.DeleteHoneyByIdAsync(id);

                TempData[WarningMessage] = "Медът беше успешно изтрит!";
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

                    // Added houses as a Beekeeper
                    myHoneys.AddRange(await honeyService.AllByBeekeeperIdAsync(beekeeperId!));

                    //this checks the admins honeys and removes the duplicates
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
