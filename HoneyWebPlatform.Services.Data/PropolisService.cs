namespace HoneyWebPlatform.Services.Data
{
    using Microsoft.EntityFrameworkCore;

    using HoneyWebPlatform.Data;
    using HoneyWebPlatform.Data.Models;
    using Interfaces;
    using Mapping;
    using Models.Propolis;
    using Models.Statistics;
    using Web.ViewModels.Beekeeper;
    using Web.ViewModels.Home;
    using Web.ViewModels.Propolis;
    using Web.ViewModels.Propolis.Enums;

    public class PropolisService : IPropolisService
    {
        private readonly HoneyWebPlatformDbContext dbContext;

        public PropolisService(HoneyWebPlatformDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<PropolisAllViewModel>> LastThreePropolisеsAsync()
        {
            IEnumerable<PropolisAllViewModel> lastThreePropolises = await dbContext
                .Propolises
                .Where(h => h.IsActive)
                .Where(h => h.IsPromoted)
                .OrderByDescending(h => h.CreatedOn)
                .Take(6)
                .To<PropolisAllViewModel>()
                .ToArrayAsync();

            return lastThreePropolises;
        }

        public async Task<string> CreateAndReturnIdAsync(PropolisFormModel formModel, string beekeeperId)
        {
            Propolis propolis = AutoMapperConfig.MapperInstance.Map<Propolis>(formModel);
            propolis.BeekeeperId = Guid.Parse(beekeeperId);
            propolis.ImageUrl = formModel.PropolisPicturePath;

            await dbContext.Propolises.AddAsync(propolis);
            await dbContext.SaveChangesAsync();

            return propolis.Id.ToString();
        }

        public async Task<AllPropolisesFilteredAndPagedServiceModel> AllAsync(AllPropolisesQueryModel queryModel)
        {
            IQueryable<Propolis> propolisesQuery = dbContext
                .Propolises
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryModel.Flavour))
            {
                propolisesQuery = propolisesQuery
                    .Where(h => h.Flavour.Name == queryModel.Flavour);
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SearchString))
            {
                string wildCard = $"%{queryModel.SearchString.ToLower()}%";

                propolisesQuery = propolisesQuery
                    .Where(h => EF.Functions.Like(h.Title, wildCard) ||
                                EF.Functions.Like(h.Description, wildCard));
            }

            propolisesQuery = queryModel.PropolisSorting switch
            {
                PropolisSorting.Newest => propolisesQuery
                    .OrderByDescending(h => h.CreatedOn),
                PropolisSorting.Oldest => propolisesQuery
                    .OrderBy(h => h.CreatedOn),
                PropolisSorting.PriceAscending => propolisesQuery
                    .OrderBy(h => h.Price),
                PropolisSorting.PriceDescending => propolisesQuery
                    .OrderByDescending(h => h.Price),
                _ => propolisesQuery
                    .OrderByDescending(h => h.CreatedOn)
            };

            IEnumerable<PropolisAllViewModel> allPropolises = await propolisesQuery
                .Where(h => h.IsActive)
                //.Skip((queryModel.CurrentPage - 1) * queryModel.PropolisesPerPage)
                //.Take(queryModel.PropolisesPerPage)
                .Select(h => new PropolisAllViewModel
                {
                    Id = h.Id.ToString(),
                    Title = h.Title,
                    ImageUrl = h.ImageUrl,
                    Price = h.Price,
                })
                .ToArrayAsync();
            int totalPropolises = propolisesQuery.Count();

            return new AllPropolisesFilteredAndPagedServiceModel()
            {
                TotalPropolisesCount = totalPropolises,
                Propolises = allPropolises
            };
        }

        public async Task<IEnumerable<PropolisAllViewModel>> AllByBeekeeperIdAsync(string beekeeperId)
        {
            IEnumerable<PropolisAllViewModel> allBeekeeperPropolises = await dbContext
                .Propolises
                .Where(h => h.IsActive &&
                            h.BeekeeperId.ToString() == beekeeperId)
                .Select(h => new PropolisAllViewModel()
                {
                    Id = h.Id.ToString(),
                    Title = h.Title,
                    ImageUrl = h.ImageUrl,
                    Price = h.Price,
                })
                .ToArrayAsync();

            return allBeekeeperPropolises;
        }

        public async Task<bool> ExistsByIdAsync(string propolisId)
        {
            bool result = await dbContext
                .Propolises
                .Where(h => h.IsActive)
                .AnyAsync(h => h.Id.ToString() == propolisId);

            return result;
        }

        public async Task<PropolisDetailsViewModel> GetDetailsByIdAsync(string propolisId)
        {
            Propolis propolis = await dbContext
                .Propolises
                .Include(h => h.Flavour)
                .Include(h => h.Beekeeper)
                .ThenInclude(a => a.User)
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == propolisId);

            return new PropolisDetailsViewModel()
            {
                Id = propolis.Id.ToString(),
                Title = propolis.Title,
                ImageUrl = propolis.ImageUrl,
                Price = propolis.Price,
                Description = propolis.Description,
                Flavour = propolis.Flavour.Name,
                Beekeeper = new BeekeeperInfoOnPropolisViewModel()
                {
                    Email = propolis.Beekeeper.User.Email,
                    PhoneNumber = propolis.Beekeeper.PhoneNumber
                }
            };
        }

        public async Task<PropolisFormModel> GetPropolisForEditByIdAsync(string propolisId)
        {
            Propolis propolis = await dbContext
                .Propolises
            .Include(h => h.Flavour)
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == propolisId);

            return new PropolisFormModel()
            {
                Title = propolis.Title,
                Description = propolis.Description,
                PropolisPicturePath = propolis.ImageUrl,
                Price = propolis.Price,
                FlavourId = propolis.FlavourId,
            };
        }

        public async Task<bool> IsBeekeeperWithIdOwnerOfPropolisWithIdAsync(string propolisId, string beekeeperId)
        {
            Propolis propolis = await dbContext
                .Propolises
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == propolisId);

            return propolis.BeekeeperId.ToString() == beekeeperId;
        }

        public async Task EditPropolisByIdAndFormModelAsync(string propolisId, PropolisFormModel formModel)
        {
            Propolis propolis = await dbContext
                .Propolises
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == propolisId);

            propolis.Title = formModel.Title;
            propolis.Description = formModel.Description;
            propolis.ImageUrl = formModel.PropolisPicturePath;
            propolis.Price = formModel.Price;
            propolis.FlavourId = formModel.FlavourId;

            await dbContext.SaveChangesAsync();
        }

        public async Task<PropolisPreDeleteDetailsViewModel> GetPropolisForDeleteByIdAsync(string propolisId)
        {
            Propolis propolis = await dbContext
                .Propolises
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == propolisId);

            return new PropolisPreDeleteDetailsViewModel()
            {
                Title = propolis.Title,
                ImageUrl = propolis.ImageUrl
            };
        }

        public async Task DeletePropolisByIdAsync(string propolisId)
        {
            Propolis propolisToDelete = await dbContext
                .Propolises
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == propolisId);

            propolisToDelete.IsActive = false;

            await dbContext.SaveChangesAsync();
        }

        public async Task TogglePromotionAsync(string propolisId)
        {
            // Retrieve the honey from the database
            Propolis propolis = await dbContext.Propolises.FirstOrDefaultAsync(p => p.Id.ToString() == propolisId);

            // Check if the honey exists and is active
            if (propolis != null && propolis.IsActive)
            {
                // Toggle the promotion status
                propolis.IsPromoted = !propolis.IsPromoted;

                // Update the honey entity in the database
                dbContext.Update(propolis);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Invalid propolis ID or propolis is not active.");
            }
        }

        public async Task<StatisticsServiceModel> GetStatisticsAsync()
        {
            return new StatisticsServiceModel()
            {
                TotalHoneys = await dbContext.Honeys.CountAsync(),
                TotalPropolises = await dbContext.Propolises.CountAsync()
            };
        }
    }
}
