namespace HoneyWebPlatform.Services.Data.Interfaces
{
    using Web.ViewModels.Flavour;

    public interface IFlavourService
    {
        Task<IEnumerable<PropolisSelectFlavourFormModel>> AllFlavoursAsync();

        Task<IEnumerable<AllFlavoursViewModel>> AllFlavoursForListAsync();

        Task<bool> ExistsByIdAsync(int id);

        Task<IEnumerable<string>> AllFlavourNamesAsync();

        Task<FlavourDetailsViewModel> GetDetailsByIdAsync(int id);
    }
}
