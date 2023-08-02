namespace HoneyWebPlatform.Services.Data.Interfaces
{
    using Web.ViewModels.Category;

    public interface ICategoryService
    {
        Task<IEnumerable<HoneySelectCategoryFormModel>> AllCategoriesAsync();

        Task<IEnumerable<AllCategoriesViewModel>> AllCategoriesForListAsync();

        Task<bool> ExistsByIdAsync(int id);

        Task<IEnumerable<string>> AllCategoryNamesAsync();

        Task<CategoryDetailsViewModel> GetDetailsByIdAsync(int id);
    }
}
