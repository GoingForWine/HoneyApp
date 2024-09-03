using HoneyWebPlatform.Web.ViewModels.Honey;
using HoneyWebPlatform.Web.ViewModels.Propolis;

namespace HoneyWebPlatform.Web.ViewModels.Home
{
    public class IndexViewModel
    {
        public IEnumerable<HoneyAllViewModel> Honeys { get; set; } = null!;
        public IEnumerable<PropolisAllViewModel> Propolises { get; set; } = null!;
        public IEnumerable<PostIndexViewModel> Posts { get; set; } = null!;

    }
}
