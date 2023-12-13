namespace HoneyWebPlatform.Services.Data.Models.Post
{
    using Web.ViewModels.Blog;

    public class AllPostsFilteredAndPagedServiceModel
    {
        public AllPostsFilteredAndPagedServiceModel()
        {
            Posts = new HashSet<PostAllViewModel>();
        }

        public int TotalPostsCount { get; set; }

        public IEnumerable<PostAllViewModel> Posts { get; set; }
    }
}