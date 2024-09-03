namespace HoneyWebPlatform.Services.Data.Interfaces
{
    using Models.Honey;
    using Models.Statistics;
    using Web.ViewModels.Home;
    using Web.ViewModels.Honey;

    public interface IHoneyService
    {
        Task<IEnumerable<HoneyAllViewModel>> LastThreeHoneysAsync();

        Task<string> CreateAndReturnIdAsync(HoneyFormModel formModel, string beekeeperId);

        Task<AllHoneysFilteredAndPagedServiceModel> AllAsync(AllHoneysQueryModel queryModel);

        Task<IEnumerable<HoneyAllViewModel>> AllByBeekeeperIdAsync(string beekeeperId);

        Task<bool> ExistsByIdAsync(string honeyId);

        Task<HoneyDetailsViewModel> GetDetailsByIdAsync(string honeyId);

        Task<HoneyFormModel> GetHoneyForEditByIdAsync(string honeyId);

        Task<bool> IsBeekeeperWithIdOwnerOfHoneyWithIdAsync(string honeyId, string beekeeperId);

        Task EditHoneyByIdAndFormModelAsync(string honeyId, HoneyFormModel formModel);

        Task<HoneyPreDeleteDetailsViewModel> GetHoneyForDeleteByIdAsync(string honeyId);

        Task DeleteHoneyByIdAsync(string honeyId);

        Task TogglePromotionAsync(string honeyId);

        Task<StatisticsServiceModel> GetStatisticsAsync();
    }
}
