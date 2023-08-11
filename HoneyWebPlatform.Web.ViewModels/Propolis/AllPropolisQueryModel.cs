namespace HoneyWebPlatform.Web.ViewModels.Propolis
{
    using System.ComponentModel.DataAnnotations;

    using Enums;

    using static Common.GeneralApplicationConstants;

    public class AllPropolisesQueryModel
    {
        public AllPropolisesQueryModel()
        {
            CurrentPage = DefaultPage;
            PropolisesPerPage = EntitiesPerPage;

            Flavours = new HashSet<string>();
            Propolises = new HashSet<PropolisAllViewModel>();
        }

        public string? Flavour { get; set; }

        [Display(Name = "Search by word")]
        public string? SearchString { get; set; }

        [Display(Name = "Sort Propolises By")]
        public PropolisSorting PropolisSorting { get; set; }

        public int CurrentPage { get; set; }

        [Display(Name = "Show Propolises On Page")]
        public int PropolisesPerPage { get; set; }

        public int TotalPropolises { get; set; }

        public IEnumerable<string> Flavours { get; set; }

        public IEnumerable<PropolisAllViewModel> Propolises { get; set; }
    }
}