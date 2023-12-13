using HoneyWebPlatform.Data.Models;

namespace HoneyWebPlatform.Services.Data.Interfaces
{
    using Web.ViewModels.Home;
    using Models.Statistics;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Web.ViewModels.Blog;
    using Models.Post;

    public interface IPostService
    {
        Task<IEnumerable<PostIndexViewModel>> LastThreePostsAsync();

        Task<string> CreateAndReturnIdAsync(PostFormModel formModel, string authorId);

        Task<AllPostsFilteredAndPagedServiceModel> AllAsync(AllPostsQueryModel queryModel);

        Task<IEnumerable<PostAllViewModel>> AllByAuthorIdAsync(string authorId);

        Task<bool> ExistsByIdAsync(string postId);

        Task<PostDetailsViewModel> GetDetailsByIdAsync(string postId);

        Task<PostFormModel> GetPostForEditByIdAsync(string postId);

        Task<bool> IsAuthorWithIdOwnerOfPostWithIdAsync(string postId, string authorId);

        Task EditPostByIdAndFormModelAsync(string postId, PostFormModel formModel);

        Task<PostPreDeleteDetailsViewModel> GetPostForDeleteByIdAsync(string postId);

        Task DeletePostByIdAsync(string postId);

        // methods for the comments

        Task AddCommentAsync(string postId, string content, string authorId);

        Task DeleteCommentByIdAsync(string commentId);

        Task<bool> CommentExistsByIdAsync(string commentId);

        Task<CommentFormModel> GetCommentForEditByIdAsync(string commentId);

        Task<bool> IsCommentAuthorWithIdOwnerOfCommentWithIdAsync(string commentId, string authorId);

        Task EditCommentByIdAndFormModelAsync(string commentId, CommentFormModel formModel);

        Task<string> GetPostIdByCommentIdAsync(string commentId);

        Task<StatisticsServiceModel> GetStatisticsAsync();
    }
}