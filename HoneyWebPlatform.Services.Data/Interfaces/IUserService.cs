﻿namespace HoneyWebPlatform.Services.Data.Interfaces
{
    using Web.ViewModels.User;

    public interface IUserService
    {
        Task<string> GetFullNameByEmailAsync(string email);

        Task<string> GetFullNameByIdAsync(string userId);

        Task<IEnumerable<UserViewModel>> AllAsync();

        Task<IEnumerable<UserViewModel>> GetSubscribedUsersAsync();

        Task<bool> DeleteUserAndRelatedEntitiesAsync(Guid userId);

    }
}
