﻿namespace HoneyWebPlatform.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;

    using Infrastructure.Extensions;

    using HoneyWebPlatform.Services.Data.Models.Post;
    using HoneyWebPlatform.Services.Data.Interfaces;

    using ViewModels.Blog;
    using ViewModels.Blog.Old;

    using static Common.GeneralApplicationConstants;
    using static Common.NotificationMessagesConstants;
    using HoneyWebPlatform.Services.Data;

    [Authorize]
    public class BlogController : Controller
    {
        private static List<KeyValuePair<string, string>> s_messages
            = new List<KeyValuePair<string, string>>();

        private readonly IPostService postService;

        private readonly IWebHostEnvironment webHostEnvironment;


        public BlogController(IPostService postService, IWebHostEnvironment webHostEnvironment)
        {
            this.postService = postService;
            this.webHostEnvironment = webHostEnvironment;
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

            //if (!ModelState.IsValid)
            //{
            //    return View(input);
            //}

            try
            {
                var authorId = User.GetId();

                // Picture saving logic
                if (input.PostPicture != null && input.PostPicture.Length > 0)
                {
                    var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "PostPictures");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + input.PostPicture.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await input.PostPicture.CopyToAsync(fileStream);
                    }

                    input.PostPicturePath = "/uploads/PostPictures/" + uniqueFileName;
                }

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


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AddComment(string postId, string content)
        {
            try
            {
                string authorId;

                if (User.Identity.IsAuthenticated)
                {
                    authorId = User.GetId(); // Get the ID of the current user
                }
                else
                {
                    // For anonymous comments, set the authorId to a default value (e.g., "Guest")
                    authorId = "347239C3-DAA6-4579-9980-44147F44A3C3";
                }

                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Details", "Blog", new { id = postId });
                }

                await postService.AddCommentAsync(postId, content, authorId);

                if (User.Identity.IsAuthenticated)
                {
                    TempData[SuccessMessage] = "Успешно добавихте коментар!";
                }
                else
                {
                    TempData[SuccessMessage] = "Успешно добавихте анонимен коментар!";
                }

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

            //if (!ModelState.IsValid)
            //{
            //    return View(input);
            //}

            // Retrieve the old honey data
            var oldPost = await postService.GetPostForEditByIdAsync(id);
            string oldPicturePath = oldPost.PostPicturePath;

            // Picture saving logic
            if (input.PostPicture != null && input.PostPicture.Length > 0)
            {
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "PostPictures");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + input.PostPicture.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await input.PostPicture.CopyToAsync(fileStream);
                }

                input.PostPicturePath = "/uploads/PostPictures/" + uniqueFileName;
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
                // Retrieve the old honey data
                var oldPost = await postService.GetPostForEditByIdAsync(id);
                string oldPicturePath = oldPost.PostPicturePath;


                // Delete the old picture file
                if (!string.IsNullOrEmpty(oldPicturePath))
                {
                    var oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, oldPicturePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

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

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> Statistics()
        //{
        //    var statistics = await postService.GetStatisticsAsync();

        //    return View(statistics);
        //}
    }
}
