namespace HoneyWebPlatform.Services.Data.Models.Honey
{
    using Web.ViewModels.Honey;

    public class AllHoneysFilteredAndPagedServiceModel
    {
        public AllHoneysFilteredAndPagedServiceModel()
        {
            Honeys = new HashSet<HoneyAllViewModel>();
        }

        public int TotalHoneysCount { get; set; }

        public IEnumerable<HoneyAllViewModel> Honeys { get; set; }
    }
}