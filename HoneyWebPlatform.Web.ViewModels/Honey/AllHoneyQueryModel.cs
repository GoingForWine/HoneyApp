namespace HoneyWebPlatform.Web.ViewModels.Honey
{
    using System.ComponentModel.DataAnnotations;

    using Enums;

    using static Common.GeneralApplicationConstants;

    public class AllHoneysQueryModel
    {
        public AllHoneysQueryModel()
        {
            CurrentPage = DefaultPage;
            HoneysPerPage = EntitiesPerPage;

            Categories = new HashSet<string>();
            Honeys = new HashSet<HoneyAllViewModel>();
        }

        public string? Category { get; set; }

        [Display(Name = "Search by word")]
        public string? SearchString { get; set; }

        [Display(Name = "Sort Honeys By")]
        public HoneySorting HoneySorting { get; set; }

        public int CurrentPage { get; set; }

        [Display(Name = "Show Honeys On Page")]
        public int HoneysPerPage { get; set; }

        public int TotalHoneys { get; set; }

        public IEnumerable<string> Categories { get; set; }

        public IEnumerable<HoneyAllViewModel> Honeys { get; set; }
    }
}