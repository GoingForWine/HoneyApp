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
    using Microsoft.AspNetCore.Hosting;
    using HoneyWebPlatform.Services.Data;


    [Authorize]
    public class PropolisController : Controller
    {
        private readonly IFlavourService flavourService;
        private readonly IBeekeeperService beekeeperService;
        private readonly IPropolisService propolisService;

        private readonly IWebHostEnvironment webHostEnvironment;


        public PropolisController(IFlavourService flavourService, IBeekeeperService beekeeperService,
            IPropolisService propolisService, IWebHostEnvironment webHostEnvironment)
        {
            this.flavourService = flavourService;
            this.beekeeperService = beekeeperService;
            this.propolisService = propolisService;
            this.webHostEnvironment = webHostEnvironment;
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
                TempData[ErrorMessage] = "Трябва да станете пчелар, за да добавяте прополиси!";

                return RedirectToAction("Become", "Beekeeper");
            }

            bool flavourExists =
                await flavourService.ExistsByIdAsync(model.FlavourId);
            if (!flavourExists)
            {
                // Adding model error to ModelState automatically makes ModelState Invalid
                ModelState.AddModelError(nameof(model.FlavourId), "Няма такъв вкус!");
                TempData[ErrorMessage] = "Няма такъв вкус!";
            }

            //if (!ModelState.IsValid)
            //{
            //    model.Flavours = await flavourService.AllFlavoursAsync();

            //    return View(model);
            //}

            try
            {
                string? beekeeperId =
                    await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);

                // Picture saving logic
                if (model.PropolisPicture != null && model.PropolisPicture.Length > 0)
                {
                    var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "PropolisPictures");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PropolisPicture.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PropolisPicture.CopyToAsync(fileStream);
                    }

                    model.PropolisPicturePath = "/uploads/PropolisPictures/" + uniqueFileName;
                }

                string propolisId =
                    await propolisService.CreateAndReturnIdAsync(model, beekeeperId!);

                TempData[SuccessMessage] = "Успешно добавихте прополиса!";
                return RedirectToAction("Details", "Propolis", new { id = propolisId });
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Неочакван проблем стана докато опитвахме да добавим Вашия прополис! Моля опитайте пак след малко или се свържете с администратор!");

                TempData[ErrorMessage] =
                    "Неочакван проблем стана докато опитвахме да добавим Вашия прополис! Моля опитайте пак след малко или се свържете с администратор!";


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
                TempData[ErrorMessage] = "Няма такъв прополис!";

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
            //if (!ModelState.IsValid)
            //{
            //    model.Flavours = await flavourService.AllFlavoursAsync();

            //    return View(model);
            //}

            bool propolisExists = await propolisService
                .ExistsByIdAsync(id);
            if (!propolisExists)
            {
                TempData[ErrorMessage] = "Няма такъв прополис!";

                return RedirectToAction("All", "Propolis");
            }

            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isUserBeekeeper && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Трябва да сте пчелар, за да редактирате!";

                return RedirectToAction("Become", "Beekeeper");
            }

            string? beekeeperId =
                await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);
            bool isBeekeeperOwner = await propolisService
                .IsBeekeeperWithIdOwnerOfPropolisWithIdAsync(id, beekeeperId!);
            if (!isBeekeeperOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Трябва да сте собственик на прополиса, за да го редактирате!";

                return RedirectToAction("Mine", "Propolis");
            }

            try
            {
                // Retrieve the old propolis data
                var oldPropolis = await propolisService.GetPropolisForEditByIdAsync(id);
                string oldPicturePath = oldPropolis.PropolisPicturePath;


                // Picture saving logic
                if (model.PropolisPicture != null && model.PropolisPicture.Length > 0)
                {
                    var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "PropolisPictures");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PropolisPicture.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PropolisPicture.CopyToAsync(fileStream);
                    }

                    model.PropolisPicturePath = "/uploads/PropolisPictures/" + uniqueFileName;
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

                await propolisService.EditPropolisByIdAndFormModelAsync(id, model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty,
                    "Неочакван проблем стана докато опитвахме да променим Вашия прополис! Моля опитайте пак след малко или се свържете с администратор!");

                TempData[ErrorMessage] =
                    "Неочакван проблем стана докато опитвахме да променим Вашия прополис! Моля опитайте пак след малко или се свържете с администратор!";


                model.Flavours = await flavourService.AllFlavoursAsync();

                return View(model);
            }

            TempData[SuccessMessage] = "Успешно редактирахте прополиса!";
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
                TempData[ErrorMessage] = "Няма такъв прополис!";

                return RedirectToAction("All", "Propolis");
            }

            bool isUserBeekeeper = await beekeeperService
                .BeekeeperExistsByUserIdAsync(User.GetId()!);
            if (!isUserBeekeeper && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Трябва да сте пчелар, за да редактирате!";

                return RedirectToAction("Become", "Beekeeper");
            }

            string? beekeeperId =
                await beekeeperService.GetBeekeeperIdByUserIdAsync(User.GetId()!);
            bool isBeekeeperOwner = await propolisService
                .IsBeekeeperWithIdOwnerOfPropolisWithIdAsync(id, beekeeperId!);
            if (!isBeekeeperOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Трябва да сте собственик на прополиса, за да го изтривате!";

                return RedirectToAction("Mine", "Propolis");
            }

            try
            {
                // Retrieve the old propolis data
                var oldPropolis = await propolisService.GetPropolisForEditByIdAsync(id);
                string oldPicturePath = oldPropolis.PropolisPicturePath;


                // Delete the old picture file
                if (!string.IsNullOrEmpty(oldPicturePath))
                {
                    var oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, oldPicturePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }


                await propolisService.DeletePropolisByIdAsync(id);

                TempData[WarningMessage] = "Успешно изтрихте прополиса!";
                return RedirectToAction("Mine", "Propolis");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty,
                    "Неочакван проблем стана докато опитвахме да изтрием Вашия прополис! Моля опитайте пак след малко или се свържете с администратор!");

                TempData[ErrorMessage] =
                    "Неочакван проблем стана докато опитвахме да изтрием Вашия прополис! Моля опитайте пак след малко или се свържете с администратор!";

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
                ModelState.AddModelError(string.Empty,
                    "Неочакван проблем стана докато опитвахме да изкараме Вашия прополис! Моля опитайте пак след малко или се свържете с администратор!");

                TempData[ErrorMessage] =
                    "Неочакван проблем стана докато опитвахме да изкараме Вашия прополис! Моля опитайте пак след малко или се свържете с администратор!";

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
