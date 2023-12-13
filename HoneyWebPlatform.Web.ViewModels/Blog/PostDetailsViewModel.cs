namespace HoneyWebPlatform.Web.ViewModels.Blog
{
    using Data.Models;

    using System.Collections.Generic;

    public class PostDetailsViewModel : PostAllViewModel
    {
        public string Content { get; set; } = null!;

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
