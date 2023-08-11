namespace HoneyWebPlatform.Web.Controllers
{
    using HoneyWebPlatform.Web.ViewModels.Blog;
    using Microsoft.AspNetCore.Mvc;

    public class BlogController : Controller
    {
        private static List<KeyValuePair<string, string>> s_messages
            = new List<KeyValuePair<string, string>>();


        public IActionResult Show()
        {
            if (s_messages.Count() < 1)
            {
                return View(new BlogViewModel());
            }
            else
            {
                var blogModel = new BlogViewModel()
                {
                    Messages = s_messages.Select(m => new MessageViewModel()
                    {
                        Sender = m.Key,
                        MessageText = m.Value
                    }).ToList()
                };

                return View(blogModel);
            }
        }

        [HttpPost]
        public IActionResult Send(BlogViewModel blog)
        {
            var newMessage = blog.CurrentMessage;

            s_messages.Add(new KeyValuePair<string, string>
                (newMessage.Sender, newMessage.MessageText));

            return RedirectToAction("Show");
        }
    }
}
