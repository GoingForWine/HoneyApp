namespace HoneyWebPlatform.Services.Data
{
    using Microsoft.EntityFrameworkCore;

    using HoneyWebPlatform.Data;
    using HoneyWebPlatform.Data.Models;
    using Interfaces;
    using Web.ViewModels.Flavour;

    public class FlavourService : IFlavourService
    {
        private readonly HoneyWebPlatformDbContext dbContext;

        public FlavourService(HoneyWebPlatformDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<PropolisSelectFlavourFormModel>> AllFlavoursAsync()
        {
            IEnumerable<PropolisSelectFlavourFormModel> allFlavours = await dbContext
                .Flavours
                .AsNoTracking()
                .Select(c => new PropolisSelectFlavourFormModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToArrayAsync();

            return allFlavours;
        }

        public async Task<IEnumerable<AllFlavoursViewModel>> AllFlavoursForListAsync()
        {
            IEnumerable<AllFlavoursViewModel> allFlavours = await dbContext
                .Flavours
                .AsNoTracking()
                .Select(c => new AllFlavoursViewModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToArrayAsync();

            return allFlavours;
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            bool result = await dbContext
                .Flavours
                .AnyAsync(c => c.Id == id);

            return result;
        }

        public async Task<IEnumerable<string>> AllFlavourNamesAsync()
        {
            IEnumerable<string> allNames = await dbContext
                .Flavours
                .Select(c => c.Name)
                .ToArrayAsync();

            return allNames;
        }

        public async Task<FlavourDetailsViewModel> GetDetailsByIdAsync(int id)
        {
            Flavour flavour = await dbContext
                .Flavours
                .FirstAsync(c => c.Id == id);

            FlavourDetailsViewModel viewModel = new FlavourDetailsViewModel()
            {
                Id = flavour.Id,
                Name = flavour.Name
            };
            return viewModel;
        }
    }
}
