namespace HoneyWebPlatform.Services.Data
{
    using Microsoft.EntityFrameworkCore;

    using HoneyWebPlatform.Data;
    using HoneyWebPlatform.Data.Models;
    using Interfaces;
    using Mapping;
    using Models.Honey;
    using Models.Statistics;
    using Web.ViewModels.Beekeeper;
    using Web.ViewModels.Home;
    using Web.ViewModels.Honey;
    using Web.ViewModels.Honey.Enums;

    public class HoneyService : IHoneyService
    {
        private readonly HoneyWebPlatformDbContext dbContext;

        public HoneyService(HoneyWebPlatformDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<HoneyAllViewModel>> LastThreeHoneysAsync()
        {
            IEnumerable<HoneyAllViewModel> lastThreeHoneys = await dbContext
                .Honeys
                .Where(h => h.IsActive)
                .Where(h => h.IsPromoted)
                .OrderByDescending(h => h.CreatedOn)
                .Take(6)
                .To<HoneyAllViewModel>()
                .ToArrayAsync();

            return lastThreeHoneys;
        }

        public async Task<string> CreateAndReturnIdAsync(HoneyFormModel formModel, string beekeeperId)
        {
            Honey newHoney = AutoMapperConfig.MapperInstance.Map<Honey>(formModel);
            newHoney.BeekeeperId = Guid.Parse(beekeeperId);
            newHoney.ImageUrl = formModel.HoneyPicturePath;

            await dbContext.Honeys.AddAsync(newHoney);
            await dbContext.SaveChangesAsync();

            return newHoney.Id.ToString();
        }

        public async Task<AllHoneysFilteredAndPagedServiceModel> AllAsync(AllHoneysQueryModel queryModel)
        {
            IQueryable<Honey> honeysQuery = dbContext
                .Honeys
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryModel.Category))
            {
                honeysQuery = honeysQuery
                    .Where(h => h.Category.Name == queryModel.Category);
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SearchString))
            {
                string wildCard = $"%{queryModel.SearchString.ToLower()}%";

                honeysQuery = honeysQuery
                    .Where(h => EF.Functions.Like(h.Title, wildCard) ||
                                EF.Functions.Like(h.Origin, wildCard) ||
                                EF.Functions.Like(h.Description, wildCard));
            }

            honeysQuery = queryModel.HoneySorting switch
            {
                HoneySorting.Newest => honeysQuery
                    .OrderByDescending(h => h.CreatedOn),
                HoneySorting.Oldest => honeysQuery
                    .OrderBy(h => h.CreatedOn),
                HoneySorting.PriceAscending => honeysQuery
                    .OrderBy(h => h.Price),
                HoneySorting.PriceDescending => honeysQuery
                    .OrderByDescending(h => h.Price),
                _ => honeysQuery
                    .OrderByDescending(h => h.CreatedOn)
            };

            IEnumerable<HoneyAllViewModel> allHoneys = await honeysQuery
                .Where(h => h.IsActive)
                //.Skip((queryModel.CurrentPage - 1) * queryModel.HoneysPerPage)
                //.Take(queryModel.HoneysPerPage)
                .Select(h => new HoneyAllViewModel
                {
                    Id = h.Id.ToString(),
                    Title = h.Title,
                    Origin = h.Origin,
                    ImageUrl = h.ImageUrl,
                    Price = h.Price,
                })
                .ToArrayAsync();
            int totalHoneys = honeysQuery.Count();

            return new AllHoneysFilteredAndPagedServiceModel()
            {
                TotalHoneysCount = totalHoneys,
                Honeys = allHoneys
            };
        }

        public async Task<IEnumerable<HoneyAllViewModel>> AllByBeekeeperIdAsync(string beekeeperId)
        {
            IEnumerable<HoneyAllViewModel> allBeekeeperHoneys = await dbContext
                .Honeys
                .Where(h => h.IsActive && h.BeekeeperId.ToString() == beekeeperId)
                .Select(h => new HoneyAllViewModel()
                {
                    Id = h.Id.ToString(),
                    Title = h.Title,
                    Origin = h.Origin,
                    ImageUrl = h.ImageUrl,
                    Price = h.Price,
                })
                .ToArrayAsync();

            return allBeekeeperHoneys;
        }

        public async Task<bool> ExistsByIdAsync(string honeyId)
        {
            bool result = await dbContext
                .Honeys
                .Where(h => h.IsActive)
                .AnyAsync(h => h.Id.ToString() == honeyId);

            return result;
        }

        public async Task<HoneyDetailsViewModel> GetDetailsByIdAsync(string honeyId)
        {
            Honey honey = await dbContext
                .Honeys
                .Include(h => h.Category)
                .Include(h => h.Beekeeper)
                .ThenInclude(a => a.User)
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == honeyId);

            return new HoneyDetailsViewModel()
            {
                Id = honey.Id.ToString(),
                Title = honey.Title,
                Origin = honey.Origin,
                ImageUrl = honey.ImageUrl,
                Price = honey.Price,
                Description = honey.Description,
                Category = honey.Category.Name,
                Beekeeper = new BeekeeperInfoOnHoneyViewModel()
                {
                    Email = honey.Beekeeper.User.Email,
                    PhoneNumber = honey.Beekeeper.PhoneNumber
                }
            };
        }

        public async Task<HoneyFormModel> GetHoneyForEditByIdAsync(string honeyId)
        {
            Honey honey = await dbContext
                .Honeys
            .Include(h => h.Category)
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == honeyId);

            return new HoneyFormModel()
            {
                Title = honey.Title,
                Origin = honey.Origin,
                Description = honey.Description,
                HoneyPicturePath = honey.ImageUrl,
                Price = honey.Price,
                CategoryId = honey.CategoryId,
            };
        }

        public async Task<bool> IsBeekeeperWithIdOwnerOfHoneyWithIdAsync(string honeyId, string beekeeperId)
        {
            Honey honey = await dbContext
                .Honeys
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == honeyId);

            return honey.BeekeeperId.ToString() == beekeeperId;
        }

        public async Task EditHoneyByIdAndFormModelAsync(string honeyId, HoneyFormModel formModel)
        {
            Honey honey = await dbContext
                .Honeys
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == honeyId);

            honey.Title = formModel.Title;
            honey.Origin = formModel.Origin;
            honey.Description = formModel.Description;
            honey.ImageUrl = formModel.HoneyPicturePath;
            honey.Price = formModel.Price;
            honey.CategoryId = formModel.CategoryId;

            await dbContext.SaveChangesAsync();
        }

        public async Task<HoneyPreDeleteDetailsViewModel> GetHoneyForDeleteByIdAsync(string honeyId)
        {
            Honey honey = await dbContext
                .Honeys
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == honeyId);

            return new HoneyPreDeleteDetailsViewModel()
            {
                Title = honey.Title,
                Origin = honey.Origin,
                ImageUrl = honey.ImageUrl
            };
        }

        public async Task DeleteHoneyByIdAsync(string honeyId)
        {
            Honey honeyToDelete = await dbContext
                .Honeys
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == honeyId);

            honeyToDelete.IsActive = false;

            await dbContext.SaveChangesAsync();
        }

        public async Task TogglePromotionAsync(string honeyId)
        {
            // Retrieve the honey from the database
            Honey honey = await dbContext.Honeys.FirstOrDefaultAsync(h => h.Id.ToString() == honeyId);

            // Check if the honey exists and is active
            if (honey != null && honey.IsActive)
            {
                // Toggle the promotion status
                honey.IsPromoted = !honey.IsPromoted;
                
                // Update the honey entity in the database
                dbContext.Update(honey);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Invalid honey ID or honey is not active.");
            }
        }


        public async Task<StatisticsServiceModel> GetStatisticsAsync()
        {
            return new StatisticsServiceModel()
            {
                TotalHoneys = await dbContext.Honeys.CountAsync(),
                TotalActiveHoneys = await dbContext.Honeys.Where(h => h.IsActive).CountAsync(),
                TotalPropolises = await dbContext.Propolises.CountAsync(),
                TotalActivePropolises = await dbContext.Propolises.Where(p => p.IsActive).CountAsync(),
                TotalPosts = await dbContext.Posts.CountAsync(),
                TotalActivePosts = await dbContext.Posts.Where(p => p.IsActive).CountAsync(),
            };
        }
    }
}
