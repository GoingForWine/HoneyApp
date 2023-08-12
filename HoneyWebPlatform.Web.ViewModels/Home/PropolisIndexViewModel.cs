
namespace HoneyWebPlatform.Web.ViewModels.Home
{
    using Services.Mapping;
    using Data.Models;

    public class PropolisIndexViewModel : IMapFrom<Propolis>
    {
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

    }
}
