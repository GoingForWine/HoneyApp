namespace HoneyWebPlatform.Services.Data
{
    using Microsoft.EntityFrameworkCore;

    using HoneyWebPlatform.Data;
    using HoneyWebPlatform.Data.Models;
    using Interfaces;
    using Web.ViewModels.Category;

    public class CategoryService : ICategoryService
    {
        private readonly HoneyWebPlatformDbContext dbContext;

        public CategoryService(HoneyWebPlatformDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<HoneySelectCategoryFormModel>> AllCategoriesAsync()
        {
            IEnumerable<HoneySelectCategoryFormModel> allCategories = await dbContext
                .Categories
                .AsNoTracking()
                .Select(c => new HoneySelectCategoryFormModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToArrayAsync();

            return allCategories;
        }

        public async Task<IEnumerable<AllCategoriesViewModel>> AllCategoriesForListAsync()
        {
            IEnumerable<AllCategoriesViewModel> allCategories = await dbContext
                .Categories
                .AsNoTracking()
                .Select(c => new AllCategoriesViewModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToArrayAsync();

            return allCategories;
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            bool result = await dbContext
                .Categories
                .AnyAsync(c => c.Id == id);

            return result;
        }

        public async Task<IEnumerable<string>> AllCategoryNamesAsync()
        {
            IEnumerable<string> allNames = await dbContext
                .Categories
                .Select(c => c.Name)
                .ToArrayAsync();

            return allNames;
        }

        public async Task<CategoryDetailsViewModel> GetDetailsByIdAsync(int id)
        {
            Category category = await dbContext
                .Categories
                .FirstAsync(c => c.Id == id);

            CategoryDetailsViewModel viewModel = new CategoryDetailsViewModel()
            {
                Id = category.Id,
                Name = category.Name
            };
            return viewModel;
        }
    }
}
