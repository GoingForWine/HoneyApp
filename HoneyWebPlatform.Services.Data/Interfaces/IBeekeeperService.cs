namespace HoneyWebPlatform.Services.Data.Interfaces
{
    using Web.ViewModels.Beekeeper;

    public interface IBeekeeperService
    {
        Task<bool> BeekeeperExistsByUserIdAsync(string userId);

        Task<bool> BeekeeperExistsByPhoneNumberAsync(string phoneNumber);

        Task Create(string userId, BecomeBeekeeperFormModel model);

        Task<string?> GetBeekeeperIdByUserIdAsync(string userId);

        Task<bool> HasHoneyWithIdAsync(string? userId, string honeyId);
    }
}
