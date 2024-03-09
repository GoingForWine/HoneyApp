namespace HoneyWebPlatform.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Hosting;

    using HoneyWebPlatform.Data;
    using HoneyWebPlatform.Data.Models;
    using Interfaces;
    using Web.ViewModels.User;

    public class UserService : IUserService
    {
        private readonly HoneyWebPlatformDbContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public UserService(HoneyWebPlatformDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            this.dbContext = dbContext;
            this.webHostEnvironment = webHostEnvironment;
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
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FirstName + " " + u.LastName,
                    IsSubscribed = u.IsSubscribed,
                })
                .ToListAsync();
            foreach (UserViewModel user in allUsers)
            {
                Beekeeper? beekeeper = this.dbContext
                    .Beekeepers
                    .FirstOrDefault(a => a.UserId == user.Id);
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

        public async Task<IEnumerable<UserViewModel>> GetSubscribedUsersAsync()
        {
            List<UserViewModel> subscribedUsers = await this.dbContext
                .Users
                .Where(u => u.IsSubscribed)
                .Select(u => new UserViewModel()
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FirstName + " " + u.LastName,
                    IsSubscribed = u.IsSubscribed,
                })
                .ToListAsync();

            foreach (UserViewModel user in subscribedUsers)
            {
                Beekeeper? beekeeper = this.dbContext
                    .Beekeepers
                    .FirstOrDefault(a => a.UserId == user.Id);
                if (beekeeper != null)
                {
                    user.PhoneNumber = beekeeper.PhoneNumber;
                }
                else
                {
                    user.PhoneNumber = string.Empty;
                }
            }

            return subscribedUsers;
        }


        public async Task<bool> DeleteUserAndRelatedEntitiesAsync(Guid userId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return false; // User not found
            }

            // Check if the user is a beekeeper
            var beekeeper = await dbContext.Beekeepers.FirstOrDefaultAsync(b => b.UserId == userId);
            if (beekeeper != null)
            {
                // Delete all honeys and propolises of the user
                var userHoneys = await dbContext.Honeys.Where(h => h.BeekeeperId == beekeeper.Id).ToListAsync();

                var userPropolises = await dbContext.Propolises.Where(p => p.BeekeeperId == beekeeper.Id).ToListAsync();

                // Remove items from all carts where these HONEYS AND PROPOLISES have been added
                var itemIdsToRemove =
                    userHoneys.Select(h => h.Id)
                    .Union(userPropolises
                        .Select(p => p.Id))
                    .ToList();

                var cartsToUpdate =
                    await dbContext.Carts
                    .Include(c => c.CartItems)
                                                         .Where(c => c.CartItems
                        .Any(i => itemIdsToRemove
                            .Contains(i.Id)))
                                                         .ToListAsync();

                foreach (var cart in cartsToUpdate)
                {
                    var cartItemsToRemove = cart.CartItems.Where(i => itemIdsToRemove.Contains(i.Id)).ToList();
                    foreach (var cartItem in cartItemsToRemove)
                    {
                        cart.CartItems.Remove(cartItem);
                    }
                }

                // Delete pictures associated with honeys and propolises
                foreach (var honey in userHoneys)
                {
                    DeletePicture(honey.ImageUrl);
                }

                foreach (var propolis in userPropolises)
                {
                    DeletePicture(propolis.ImageUrl);
                }

                DeletePicture(beekeeper.HiveFarmPicturePaths);

                dbContext.Honeys.RemoveRange(userHoneys);
                dbContext.Propolises.RemoveRange(userPropolises);

            }

            // Delete posts and associated comments of the user
            var userPosts = await dbContext.Posts.Include(p => p.Comments)
                                                  .Where(p => p.AuthorId == userId)
                                                  .ToListAsync();
            foreach (var post in userPosts)
            {
                dbContext.Comments.RemoveRange(post.Comments);
                DeletePicture(post.ImageUrl);
                dbContext.Posts.Remove(post);
            }

            // Delete comments made by the user
            var userComments = await dbContext.Comments.Where(c => c.AuthorId == userId).ToListAsync();
            dbContext.Comments.RemoveRange(userComments);

            var userOrders = await dbContext.Orders.Where(o => o.UserId == userId).ToListAsync();
            dbContext.Orders.RemoveRange(userOrders);

            if (user.ProfilePicturePath != null)
            {
                DeletePicture(user.ProfilePicturePath);
            }

            // Save changes to the database
            await dbContext.SaveChangesAsync();

            return true; // User and related entities deleted successfully
        }

        private void DeletePicture(string picturePath)
        {
            if (!string.IsNullOrEmpty(picturePath))
            {
                var filePath = Path.Combine(webHostEnvironment.WebRootPath, picturePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }



    }
}
