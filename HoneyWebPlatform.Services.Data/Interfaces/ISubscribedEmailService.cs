namespace HoneyWebPlatform.Services.Data.Interfaces
{
    using HoneyWebPlatform.Data.Models;

    using System.Collections.Generic;

    using System.Threading.Tasks;

    public interface ISubscribedEmailService
    {
        Task<IEnumerable<SubscribedEmail>> GetSubscribedEmailsAsync();

        Task AddSubscribedEmailAsync(string email);

        Task RemoveSubscribedEmailAsync(string email);

        Task<bool> IsEmailSubscribedAsync(string email);

    }
}
