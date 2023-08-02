namespace HoneyWebPlatform.Web.ViewModels.Honey
{
    using Beekeeper;

    public class HoneyDetailsViewModel : HoneyAllViewModel
    {
        public string Description { get; set; } = null!;

        public string Category { get; set; } = null!;

        public BeekeeperInfoOnHoneyViewModel Beekeeper { get; set; } = null!;
    }
}
