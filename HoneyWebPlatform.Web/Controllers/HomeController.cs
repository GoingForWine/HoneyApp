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
        private readonly IPostService postService;


        private readonly IEmailSender emailSender;


        public HomeController(IHoneyService honeyService, IPropolisService propolisService, IPostService postService, IEmailSender emailSender)
        {
            this.honeyService = honeyService;
            this.propolisService = propolisService;
            this.postService = postService;
            this.emailSender = emailSender;
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

            IEnumerable<PostIndexViewModel> postIndexViewModel =
                await postService.LastThreePostsAsync();

            var viewModel = new IndexViewModel
            {
                Honeys = honeyIndexViewModel,
                Propolises = propolisIndexViewModel,
                Posts = postIndexViewModel
            };


            return View(viewModel);
        }

        public IActionResult Contact()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SendEmail(string name, string email, string subject, string message)
        {
            try
            {
                // Construct the email message body
                var emailMessage = $"Name: {name}\nEmail: {email}\nSubject: {subject}\nMessage: {message}";

                // Use the EmailSender service to send the email
                await emailSender.SendEmailAsync("savethebee2024@gmail.com", subject, emailMessage);

                // Optionally, redirect to a success page or return a success message
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., log the error, display error message to user)
                return RedirectToAction("Error", "Home");
            }
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