namespace HoneyWebPlatform.Web.ViewModels.Home
{
    public class IndexViewModel
    {
        public IEnumerable<HoneyIndexViewModel> Honeys { get; set; } = null!;
        public IEnumerable<PropolisIndexViewModel> Propolises { get; set; } = null!;
        public IEnumerable<PostIndexViewModel> Posts { get; set; } = null!;

    }
}
