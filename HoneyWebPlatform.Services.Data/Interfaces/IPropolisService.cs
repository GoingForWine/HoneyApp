namespace HoneyWebPlatform.Services.Data.Interfaces
{
    using Models.Propolis;
    using Models.Statistics;
    using Web.ViewModels.Home;
    using Web.ViewModels.Propolis;

    public interface IPropolisService
    {
        Task<IEnumerable<PropolisAllViewModel>> LastThreePropolisеsAsync();

        Task<string> CreateAndReturnIdAsync(PropolisFormModel formModel, string beekeeperId);

        Task<AllPropolisesFilteredAndPagedServiceModel> AllAsync(AllPropolisesQueryModel queryModel);

        Task<IEnumerable<PropolisAllViewModel>> AllByBeekeeperIdAsync(string beekeeperId);

        Task<bool> ExistsByIdAsync(string propolisId);

        Task<PropolisDetailsViewModel> GetDetailsByIdAsync(string propolisId);

        Task<PropolisFormModel> GetPropolisForEditByIdAsync(string propolisId);

        Task<bool> IsBeekeeperWithIdOwnerOfPropolisWithIdAsync(string propolisId, string beekeeperId);

        Task EditPropolisByIdAndFormModelAsync(string propolisId, PropolisFormModel formModel);

        Task<PropolisPreDeleteDetailsViewModel> GetPropolisForDeleteByIdAsync(string propolisId);

        Task DeletePropolisByIdAsync(string propolisId);

        Task TogglePromotionAsync(string propolisId);


        //Task<StatisticsServiceModel> GetStatisticsAsync();
    }
}
