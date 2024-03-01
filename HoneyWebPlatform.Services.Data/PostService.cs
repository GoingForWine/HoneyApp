using HoneyWebPlatform.Services.Data.Models.Post;
using HoneyWebPlatform.Web.ViewModels.Blog.Enums;

namespace HoneyWebPlatform.Services.Data
{
    using Web.ViewModels.Home;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HoneyWebPlatform.Data;
    using HoneyWebPlatform.Data.Models;
    using Interfaces;
    using Mapping;
    using Models.Statistics;
    using Microsoft.EntityFrameworkCore;
    using Web.ViewModels.Blog;

    public class PostService : IPostService
    {
        private readonly HoneyWebPlatformDbContext dbContext;

        public PostService(HoneyWebPlatformDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<PostIndexViewModel>> LastThreePostsAsync()
        {
            var lastThreePosts = await dbContext.Posts
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedOn)
                .Take(3)
                .To<PostIndexViewModel>()
                .ToArrayAsync();

            return lastThreePosts;
        }

        public async Task<string> CreateAndReturnIdAsync(PostFormModel formModel, string authorId)
        {
            var newPost = AutoMapperConfig.MapperInstance.Map<Post>(formModel);
            newPost.AuthorId = Guid.Parse(authorId);
            newPost.ImageUrl = formModel.PostPicturePath;

            await dbContext.Posts.AddAsync(newPost);
            await dbContext.SaveChangesAsync();

            return newPost.Id.ToString();
        }

        public async Task<AllPostsFilteredAndPagedServiceModel> AllAsync(AllPostsQueryModel queryModel)
        {
            IQueryable<Post> postsQuery = dbContext
                .Posts
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryModel.SearchString))
            {
                string wildCard = $"%{queryModel.SearchString.ToLower()}%";

                postsQuery = postsQuery
                    .Where(p => EF.Functions.Like(p.Title, wildCard)
                                || EF.Functions.Like(p.Content, wildCard));
            }

            postsQuery = queryModel.PostSorting switch
            {
                PostSorting.Newest => postsQuery
                    .OrderByDescending(h => h.CreatedOn),
                PostSorting.Oldest => postsQuery
                    .OrderBy(h => h.CreatedOn),
                _ => postsQuery
                    .OrderByDescending(h => h.CreatedOn)
            };

            IEnumerable<PostAllViewModel> allPosts = await postsQuery
                .Where(p => p.IsActive)
                //.Skip((queryModel.CurrentPage - 1) * queryModel.PostsPerPage)
                //.Take(queryModel.PostsPerPage)
                .Select(p => new PostAllViewModel
                {
                    CreatedOn = p.CreatedOn,
                    IsActive = p.IsActive,
                    Author = p.Author,
                    Id = p.Id.ToString(),
                    Title = p.Title,
                    ImageUrl = p.ImageUrl,
                    CommentsCount = p.Comments.Count(c => c.IsActive)
                })
                .ToArrayAsync();
            int totalPosts = postsQuery.Count();

            return new AllPostsFilteredAndPagedServiceModel()
            {
                TotalPostsCount = totalPosts,
                Posts = allPosts
            };
        }

        public async Task<IEnumerable<PostAllViewModel>> AllByAuthorIdAsync(string authorId)
        {
            var allAuthorPosts = await dbContext
                .Posts
                .Where(p => p.IsActive && p.AuthorId.ToString() == authorId)
                .Select(p => new PostAllViewModel
                {
                    CreatedOn = p.CreatedOn,
                    IsActive = p.IsActive,
                    Author = p.Author,
                    Id = p.Id.ToString(),
                    Title = p.Title,
                    ImageUrl = p.ImageUrl,
                })
                .ToArrayAsync();

            return allAuthorPosts;
        }

        public async Task<bool> ExistsByIdAsync(string postId)
        {
            return await dbContext.Posts
                .Where(p => p.IsActive)
                .AnyAsync(p => p.Id.ToString() == postId);
        }

        public async Task<PostDetailsViewModel> GetDetailsByIdAsync(string postId)
        {
            var postDetails = await dbContext
                .Posts
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .Where(p => p.IsActive)
                .FirstOrDefaultAsync(p => p.Id.ToString() == postId);

            if (postDetails == null)
            {
                // Handle the case where the post with the given ID is not found.
                Console.WriteLine($"Post with ID {postId} not found.");

                return null; // Or throw an exception or return a default PostDetailsViewModel.
            }
            Console.WriteLine($"Post with ID {postId} found.");

            return new PostDetailsViewModel()
            {
                Id = postDetails.Id.ToString(),
                Title = postDetails.Title,
                ImageUrl = postDetails.ImageUrl,
                Author = postDetails.Author,
                Comments = postDetails.Comments,
                CreatedOn = postDetails.CreatedOn,
                IsActive = postDetails.IsActive,
                Content = postDetails.Content,
            };
        }

        public async Task<PostFormModel> GetPostForEditByIdAsync(string postId)
        {
            var postForEdit = await dbContext.Posts
                .Where(p => p.IsActive)
                .FirstOrDefaultAsync(p => p.Id.ToString() == postId);

            return new PostFormModel()
            {
                Title = postForEdit.Title,
                PostPicturePath = postForEdit.ImageUrl,
                Content = postForEdit.Content,
            };
        }

        public async Task<bool> IsAuthorWithIdOwnerOfPostWithIdAsync(string postId, string authorId)
        {
            return await dbContext.Posts
                .Where(p => p.IsActive)
                .AnyAsync(p => p.Id.ToString() == postId && p.AuthorId.ToString() == authorId);
        }

        public async Task EditPostByIdAndFormModelAsync(string postId, PostFormModel formModel)
        {
            var postToEdit = await dbContext.Posts
                .Where(p => p.IsActive)
                .FirstOrDefaultAsync(p => p.Id.ToString() == postId);

            postToEdit.Title = formModel.Title;
            postToEdit.Content = formModel.Content;
            postToEdit.ImageUrl = formModel.PostPicturePath;

            await dbContext.SaveChangesAsync();
        }

        public async Task<PostPreDeleteDetailsViewModel> GetPostForDeleteByIdAsync(string postId)
        {
            var postToDelete = await dbContext
                .Posts
                .Where(p => p.IsActive)
                .FirstOrDefaultAsync(p => p.Id.ToString() == postId);

            return new PostPreDeleteDetailsViewModel()
            {
                Title = postToDelete.Title,
                Content = postToDelete.Content,
                ImageUrl = postToDelete.ImageUrl
            };
        }

        public async Task DeletePostByIdAsync(string postId)
        {
            var postToDelete = await dbContext.Posts
                .Where(p => p.IsActive)
                .FirstOrDefaultAsync(p => p.Id.ToString() == postId);

            postToDelete.IsActive = false;

            await dbContext.SaveChangesAsync();
        }

        public async Task AddCommentAsync(string postId, string content, string authorId)
        {
            var post = await dbContext.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id.ToString() == postId && p.IsActive);

            if (post == null)
            {
                // Handle the case where the post with the given ID is not found.
                throw new InvalidOperationException($"Post with ID {postId} not found.");
            }

            
            var comment = new Comment
            {
                Content = content,
                AuthorId = Guid.Parse(authorId),
                CreatedOn = DateTime.Now,
                IsActive = true,
                ParentPostId = post.Id
            };

            await dbContext.Comments.AddAsync(comment);
            post.Comments.Add(comment);
            await dbContext.SaveChangesAsync();
        }



        public async Task DeleteCommentByIdAsync(string commentId)
        {
            var commentToDelete = await dbContext.Comments
                .Where(c => c.IsActive)
                .FirstOrDefaultAsync(c => c.Id.ToString() == commentId);

            commentToDelete.IsActive = false;

            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> CommentExistsByIdAsync(string commentId)
        {
            return await dbContext
                .Comments
                .Where(c => c.IsActive)
                .AnyAsync(c => c.Id.ToString() == commentId);
        }

        public async Task<CommentFormModel> GetCommentForEditByIdAsync(string commentId)
        {
            var comment = await this.dbContext.Comments
                .Where(c => c.Id.ToString() == commentId && c.IsActive)
                .Select(c => new CommentFormModel
                {
                    Content = c.Content,
                })
                .FirstOrDefaultAsync();

            return comment;
        }

        public async Task<bool> IsCommentAuthorWithIdOwnerOfCommentWithIdAsync(string commentId, string authorId)
        {
            return await this.dbContext
                .Comments
                .Where(c => c.IsActive)
                .AnyAsync(c => c.Id.ToString() == commentId && c.AuthorId.ToString() == authorId);
        }

        public async Task EditCommentByIdAndFormModelAsync(string commentId, CommentFormModel formModel)
        {
            var comment = await this.dbContext
                .Comments
                .Where(c => c.IsActive)
                .FirstOrDefaultAsync(c => c.Id.ToString() == commentId);

            if (comment != null)
            {
                comment.Content = formModel.Content;
                // Optionally, you may update other properties if needed.

                this.dbContext.Update(comment);
                await this.dbContext.SaveChangesAsync();
            }
        }

        public async Task<string> GetPostIdByCommentIdAsync(string commentId)
        {
            var postId = await this.dbContext.Comments
                .Where(c => c.Id.ToString() == commentId && c.IsActive)
                .Select(c => c.ParentPostId.ToString())
                .FirstOrDefaultAsync();

            return postId.ToString();
        }

        //public async Task<StatisticsServiceModel> GetStatisticsAsync()
        //{
        //    return new StatisticsServiceModel()
        //    {
        //        TotalHoneys = await dbContext.Honeys.CountAsync(),
        //        TotalActiveHoneys = await dbContext.Honeys.Where(h => h.IsActive).CountAsync(),
        //        TotalPropolises = await dbContext.Propolises.CountAsync(),
        //        TotalActivePropolises = await dbContext.Propolises.Where(p => p.IsActive).CountAsync(),
        //        TotalPosts = await dbContext.Posts.CountAsync(),
        //    };
        //}
    }
}
