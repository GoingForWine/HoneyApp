namespace HoneyWebPlatform.Web.ViewModels.Blog
{
    using Enums;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Common.GeneralApplicationConstants;

    public class AllPostsQueryModel
    {
        public AllPostsQueryModel()
        {
            CurrentPage = DefaultPage;
            PostsPerPage = EntitiesPerPage;

            Posts = new List<PostAllViewModel>();
        }

        [Display(Name = "Sort Posts By")]
        public PostSorting PostSorting { get; set; }

        [Display(Name = "Search by Title")]
        public string? SearchString { get; set; }

        public int CurrentPage { get; set; }

        [Display(Name = "Show Posts On Page")]
        public int PostsPerPage { get; set; }

        public int TotalPosts { get; set; }

        public IEnumerable<PostAllViewModel> Posts { get; set; }
    }
}
