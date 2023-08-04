namespace HoneyWebPlatform.Services.Data
{
    using Microsoft.EntityFrameworkCore;

    using HoneyWebPlatform.Data;
    using HoneyWebPlatform.Data.Models;
    using Interfaces;
    using Web.ViewModels.Beekeeper;

    public class BeekeeperService : IBeekeeperService
    {
        private readonly HoneyWebPlatformDbContext dbContext;

        public BeekeeperService(HoneyWebPlatformDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> BeekeeperExistsByUserIdAsync(string userId)
        {
            bool result = await dbContext
                .Beekeepers
                .AnyAsync(a => a.UserId.ToString() == userId);

            return result;
        }

        public async Task<string> BeekeeperFullnameByHoneyIdAsync(string honeyId)
        {
            Honey? honey = await dbContext
                .Honeys
                .FirstOrDefaultAsync(h => h.Id.ToString() == honeyId);

            Beekeeper beekeeper = honey!.Beekeeper;

            ApplicationUser? user = await dbContext
                .Users
                .FirstOrDefaultAsync(a => a.Id.ToString() == beekeeper.UserId.ToString());

            return $"{user!.FirstName} {user!.LastName}";
        }

        public async Task<bool> BeekeeperExistsByPhoneNumberAsync(string phoneNumber)
        {
            bool result = await dbContext
                .Beekeepers
                .AnyAsync(a => a.PhoneNumber == phoneNumber);

            return result;
        }

        public async Task Create(string userId, BecomeBeekeeperFormModel model)
        {
            Beekeeper newBeekeeper = new Beekeeper()
            {
                PhoneNumber = model.PhoneNumber,
                UserId = Guid.Parse(userId)
            };

            await dbContext.Beekeepers.AddAsync(newBeekeeper);
            await dbContext.SaveChangesAsync();
        }

        public async Task<string?> GetBeekeeperIdByUserIdAsync(string userId)
        {
            Beekeeper? beekeeper = await dbContext
                .Beekeepers
                .FirstOrDefaultAsync(a => a.UserId.ToString() == userId);
            if (beekeeper == null)
            {
                return null;
            }

            return beekeeper.Id.ToString();
        }

        public async Task<bool> HasHoneyWithIdAsync(string? userId, string honeyId)
        {
            Beekeeper? beekeeper = await dbContext
                .Beekeepers
                .Include(a => a.OwnedHoney)
                .FirstOrDefaultAsync(a => a.UserId.ToString() == userId);
            if (beekeeper == null)
            {
                return false;
            }

            honeyId = honeyId.ToLower();
            return beekeeper.OwnedHoney.Any(h => h.Id.ToString() == honeyId);
        }
    }
}
