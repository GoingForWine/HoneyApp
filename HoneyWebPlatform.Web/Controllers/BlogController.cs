using AutoMapper.QueryableExtensions;
using HoneyWebPlatform.Services.Data.Models.Post;

namespace HoneyWebPlatform.Web.Controllers
{
    using ViewModels.Blog;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using HoneyWebPlatform.Web.ViewModels.Blog.Old;

    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using HoneyWebPlatform.Common;
    using Infrastructure.Extensions;

    using HoneyWebPlatform.Services.Data;
    using HoneyWebPlatform.Web.ViewModels.Blog;
    using HoneyWebPlatform.Services.Data.Interfaces;

    using static Common.GeneralApplicationConstants;
    using static Common.NotificationMessagesConstants;
    using HoneyWebPlatform.Web.ViewModels.Honey;

    [Authorize]
    public class BlogController : Controller
    {
        private static List<KeyValuePair<string, string>> s_messages
            = new List<KeyValuePair<string, string>>();

        private readonly IPostService postService;

        public BlogController(IPostService postService)
        {
            this.postService = postService;
        }
        public IActionResult ShowOld()
        {
            if (s_messages.Count() < 1)
            {
                return View(new BlogViewModelOld());
            }
            else
            {
                var blogModel = new BlogViewModelOld()
                {
                    Messages = s_messages.Select(m => new MessageViewModelOld()
                    {
                        Sender = m.Key,
                        MessageText = m.Value
                    }).ToList()
                };

                return View(blogModel);
            }

        }

        [HttpPost]
        public IActionResult Send(BlogViewModelOld blog)
        {
            var newMessage = blog.CurrentMessage;

            s_messages.Add(new KeyValuePair<string, string>
                (newMessage.Sender, newMessage.MessageText));

            return RedirectToAction("ShowOld");
        }

        ///

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All([FromQuery] AllPostsQueryModel query)
        {
            AllPostsFilteredAndPagedServiceModel viewModel =
                await postService.AllAsync(query);

            query.Posts = viewModel.Posts;
            query.TotalPosts = viewModel.TotalPostsCount;

            return View(query);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            bool isUser = User.Identity.IsAuthenticated;

            if (!isUser)
            {
                TempData[ErrorMessage] = "Трябва да влезете във вашият профил, за да направите пост!";
                return RedirectToAction("All");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(PostFormModel input)
        {
            bool isUser = User.Identity.IsAuthenticated;

            if (!isUser)
            {
                TempData[ErrorMessage] = "Трябва да влезете във вашият профил, за да направите пост!";
                return RedirectToAction("All");
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            try
            {
                var authorId = User.GetId();
                var postId = await postService.CreateAndReturnIdAsync(input, authorId!);

                TempData[SuccessMessage] = "Успешно добавихте пост!";

                return RedirectToAction("Details", "Blog", new { id = postId });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty,
                    "Неочакван проблем стана докато опитвахме да добавим Вашия мед! " +
                    "Моля опитайте пак след малко или се свържете с администратор!");

                return View(input);
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(string postId, string content)
        {
            bool isUser = User.Identity.IsAuthenticated;

            if (!isUser)
            {
                TempData[ErrorMessage] = "Трябва да влезете във вашият профил, за да направите пост!";
                return RedirectToAction("Details", new { id = postId });
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Blog", new { id = postId });
            }


            try
            {
                var authorId = User.GetId(); // Get the ID of the current user

                await postService.AddCommentAsync(postId, content, authorId);

                TempData[SuccessMessage] = "Успешно добавихте коментар!";
                Console.WriteLine("____________________________________________________________________________123");

                // Redirect back to the post details page with the added comment
                return RedirectToAction("Details", "Blog", new { id = postId });
            }
            catch (Exception e)
            {
                // Handle exceptions, perhaps redirect back to the details page with an error message
                TempData[ErrorMessage] = "Неочакван проблем стана докато опитвахме да добавим коментар! " +
                                         "Моля опитайте пак след малко или се свържете с администратор!";

                return RedirectToAction("Details", new { id = postId, error = e.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            if (!await postService.ExistsByIdAsync(id))
            {
                TempData[ErrorMessage] = "Няма такъв пост!";
                Console.WriteLine($"Post with ID {id} not found.");
                return RedirectToAction("All", "Blog");
            }

            var viewModel = await postService.GetDetailsByIdAsync(id);

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (!await postService.ExistsByIdAsync(id))
            {
                TempData[ErrorMessage] = "Няма такъв пост!";

                return RedirectToAction("All", "Blog");
            }

            var isAuthor = await postService.IsAuthorWithIdOwnerOfPostWithIdAsync(id, User.GetId());

            if (!isAuthor && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Нямате право да променяте този пост!";

                return RedirectToAction("All", "Blog");
            }

            var postForEdit = await postService.GetPostForEditByIdAsync(id);

            return View(postForEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, PostFormModel input)
        {
            if (!await postService.ExistsByIdAsync(id))
            {
                TempData[ErrorMessage] = "Няма такъв пост!";

                return RedirectToAction("All", "Blog");
            }

            var isAuthor = await postService.IsAuthorWithIdOwnerOfPostWithIdAsync(id, User.GetId()!);

            if (!isAuthor && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Нямате право да променяте поста!";

                return RedirectToAction("All", "Blog");
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            await postService.EditPostByIdAndFormModelAsync(id, input);

            TempData[WarningMessage] = "Постът беше успешно променен!";

            return RedirectToAction("Details", "Blog", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await postService.ExistsByIdAsync(id))
            {
                TempData[ErrorMessage] = "Няма такъв пост!";

                return RedirectToAction("All", "Blog");
            }

            var isAuthor = await postService.IsAuthorWithIdOwnerOfPostWithIdAsync(id, User.GetId());

            if (!isAuthor && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Нямате право да променяте поста!";

                return RedirectToAction("All", "Blog");
            }

            var postForDelete = await postService.GetPostForDeleteByIdAsync(id);

            return View(postForDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, PostPreDeleteDetailsViewModel model)
        {
            if (!await postService.ExistsByIdAsync(id))
            {
                TempData[ErrorMessage] = "Няма такъв пост!";

                return NotFound();
            }

            var isAuthor = await postService.IsAuthorWithIdOwnerOfPostWithIdAsync(id, User.GetId()!);

            if (!isAuthor && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Нямате право да променяте поста!";
                return RedirectToAction("All", "Blog");
            }

            try
            {
                await postService.DeletePostByIdAsync(id);

                TempData[WarningMessage] = "Постът беше успешно изтрит!";
                return RedirectToAction("All", "Blog");
            }
            catch (Exception e)
            {
                // Handle exceptions, perhaps redirect back to the details page with an error message
                TempData[ErrorMessage] = "Неочакван проблем стана докато опитвахме да добавим коментар! " +
                                         "Моля опитайте пак след малко или се свържете с администратор!";

                return RedirectToAction("Details", new { id = id, error = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Mine()
        {
            if (this.User.IsInRole(AdminRoleName))
            {
                return this.RedirectToAction("Mine", "Blog", new { Area = AdminAreaName });
            }

            bool isUserAuthenticated = User.Identity.IsAuthenticated;

            if (!isUserAuthenticated)
            {
                TempData[ErrorMessage] = "Трябва да влезете във вашият профил, за да видите своите постове!";
                return RedirectToAction("All", "Blog");
            }

            string userId = User.GetId();
            IEnumerable<PostAllViewModel> userPosts = await postService.AllByAuthorIdAsync(userId);

            return View(userPosts);
        }

        [HttpGet]
        public async Task<IActionResult> EditComment(string commentId)
        {
            if (!await postService.CommentExistsByIdAsync(commentId))
            {
                TempData[ErrorMessage] = "Няма такъв коментар!";
                return RedirectToAction("All", "Blog");
            }

            var isCommentAuthor = await postService.IsCommentAuthorWithIdOwnerOfCommentWithIdAsync(commentId, User.GetId());

            if (!isCommentAuthor && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Нямате право да редактирате този коментар!";
                return RedirectToAction("All", "Blog");
            }

            var commentForEdit = await postService.GetCommentForEditByIdAsync(commentId);

            return View(commentForEdit);
        }

        [HttpPost]
        public async Task<IActionResult> EditComment(string commentId, CommentFormModel input)
        {
            if (!await postService.CommentExistsByIdAsync(commentId))
            {
                TempData[ErrorMessage] = "Няма такъв коментар!";
                return RedirectToAction("All", "Blog");
            }

            var isCommentAuthor = await postService.IsCommentAuthorWithIdOwnerOfCommentWithIdAsync(commentId, User.GetId());

            if (!isCommentAuthor && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Нямате право да редактирате този коментар!";
                return RedirectToAction("All", "Blog");
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            await postService.EditCommentByIdAndFormModelAsync(commentId, input);

            TempData[WarningMessage] = "Коментарът беше успешно редактиран!";

            // Get the associated post ID for redirection
            var postId = await postService.GetPostIdByCommentIdAsync(commentId);

            return RedirectToAction("Details", "Blog", new { id = postId });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteComment(string commentId)
        {
            if (!await postService.CommentExistsByIdAsync(commentId))
            {
                TempData[ErrorMessage] = "Няма такъв коментар!";
                return RedirectToAction("All", "Blog");
            }

            var isCommentAuthor = await postService.IsCommentAuthorWithIdOwnerOfCommentWithIdAsync(commentId, User.GetId());

            if (!isCommentAuthor && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Нямате право да изтривате този коментар!";
                return RedirectToAction("All", "Blog");
            }

            try
            {
                // Get the associated post ID for redirection
                var postId = await postService.GetPostIdByCommentIdAsync(commentId);

                await postService.DeleteCommentByIdAsync(commentId);

                TempData[WarningMessage] = "Коментарът беше успешно изтрит!";

                // Redirect back to the post details page
                return RedirectToAction("Details", "Blog", new { id = postId });
            }
            catch (Exception e)
            {
                // Handle exceptions, perhaps redirect back to the details page with an error message
                TempData[ErrorMessage] = "Неочакван проблем стана докато опитвахме да изтрием коментар! " +
                                         "Моля опитайте пак след малко или се свържете с администратор!";

                return RedirectToAction("Details");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Statistics()
        {
            var statistics = await postService.GetStatisticsAsync();

            return View(statistics);
        }
    }
}
