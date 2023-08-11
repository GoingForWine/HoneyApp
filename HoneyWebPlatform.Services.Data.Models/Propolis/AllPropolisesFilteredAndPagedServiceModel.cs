namespace HoneyWebPlatform.Services.Data.Models.Propolis
{
    using Web.ViewModels.Propolis;

    public class AllPropolisesFilteredAndPagedServiceModel
    {
        public AllPropolisesFilteredAndPagedServiceModel()
        {
            Propolises = new HashSet<PropolisAllViewModel>();
        }

        public int TotalPropolisesCount { get; set; }

        public IEnumerable<PropolisAllViewModel> Propolises { get; set; }
    }
}