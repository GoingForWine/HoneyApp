using Microsoft.AspNetCore.Mvc;

namespace HoneyWebPlatform.Web.Areas.Admin.Controllers
{
    public class BaseAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
