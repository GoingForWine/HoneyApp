namespace HoneyWebPlatform.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Infrastructure.Extensions;
    using Services.Data.Interfaces;
    using ViewModels.Beekeeper;

    using static Common.NotificationMessagesConstants;

    [Authorize]
    public class BeekeeperController : Controller
    {
        private readonly IBeekeeperService beekeeperService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public BeekeeperController(IBeekeeperService beekeeperService, IWebHostEnvironment webHostEnvironment)
        {
            this.beekeeperService = beekeeperService;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Become()
        {
            string? userId = User.GetId();
            bool isBeekeeper = await beekeeperService.BeekeeperExistsByUserIdAsync(userId!);
            if (isBeekeeper)
            {
                TempData[ErrorMessage] = "You are already a Beekeeper!";

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Become(BecomeBeekeeperFormModel model)
        {
            string? userId = User.GetId();
            bool isBeekeeper = await beekeeperService.BeekeeperExistsByUserIdAsync(userId!);
            if (isBeekeeper)
            {
                TempData[ErrorMessage] = "You are already a Beekeeper!";
                return RedirectToAction("Index", "Home");
            }

            bool isPhoneNumberTaken =
                await beekeeperService.BeekeeperExistsByPhoneNumberAsync(model.PhoneNumber);
            if (isPhoneNumberTaken)
            {
                ModelState.AddModelError(nameof(model.PhoneNumber),
                    "A Beekeeper with the provided phone number already exists!");
            }

            try
            {
                // Picture saving logic
                if (model.HivePicture != null && model.HivePicture.Length > 0)
                {
                    var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "HivePictures");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.HivePicture.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.HivePicture.CopyToAsync(fileStream);
                    }

                    model.HivePicturePath = "/uploads/HivePictures/" + uniqueFileName;
                }

                await beekeeperService.Create(userId!, model);
            }
            catch (Exception ex)
            {
                TempData[ErrorMessage] =
                    "Unexpected error occurred while registering you as a Beekeeper! Please try again later or contact administrator.";

                return RedirectToAction("Index", "Home");
            }


            return RedirectToAction("All", "Honey");
        }


    }
}
