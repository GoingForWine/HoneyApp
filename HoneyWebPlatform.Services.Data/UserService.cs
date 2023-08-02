namespace HoneyWebPlatform.Services.Data
{
    using Microsoft.EntityFrameworkCore;

    using HoneyWebPlatform.Data;
    using HoneyWebPlatform.Data.Models;
    using Interfaces;
    using Web.ViewModels.User;

    public class UserService : IUserService
    {
        private readonly HoneyWebPlatformDbContext dbContext;

        public UserService(HoneyWebPlatformDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> GetFullNameByEmailAsync(string email)
        {
            ApplicationUser? user = await this.dbContext
                .Users
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return string.Empty;
            }

            return $"{user.FirstName} {user.LastName}";
        }

        public async Task<string> GetFullNameByIdAsync(string userId)
        {
            ApplicationUser? user = await this.dbContext
                .Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return string.Empty;
            }

            return $"{user.FirstName} {user.LastName}";
        }

        public async Task<IEnumerable<UserViewModel>> AllAsync()
        {
            List<UserViewModel> allUsers = await this.dbContext
                .Users
                .Select(u => new UserViewModel()
                {
                    Id = u.Id.ToString(),
                    Email = u.Email,
                    FullName = u.FirstName + " " + u.LastName
                })
                .ToListAsync();
            foreach (UserViewModel user in allUsers)
            {
                Beekeeper? beekeeper = this.dbContext
                    .Beekeepers
                    .FirstOrDefault(a => a.UserId.ToString() == user.Id);
                if (beekeeper != null)
                {
                    user.PhoneNumber = beekeeper.PhoneNumber;
                }
                else
                {
                    user.PhoneNumber = string.Empty;
                }
            }

            return allUsers;
        }
    }
}
