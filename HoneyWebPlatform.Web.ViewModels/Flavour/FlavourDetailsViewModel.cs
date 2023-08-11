namespace HoneyWebPlatform.Web.ViewModels.Flavour
{
    using Interfaces;

    public class FlavourDetailsViewModel : IFlavourDetailsModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }
}
