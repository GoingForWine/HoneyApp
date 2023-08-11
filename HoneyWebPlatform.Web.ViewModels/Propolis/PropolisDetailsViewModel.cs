namespace HoneyWebPlatform.Web.ViewModels.Propolis
{
    using Beekeeper;

    public class PropolisDetailsViewModel : PropolisAllViewModel
    {
        public string Description { get; set; } = null!;

        public string Flavour { get; set; } = null!;

        public BeekeeperInfoOnPropolisViewModel Beekeeper { get; set; } = null!;
    }
}
