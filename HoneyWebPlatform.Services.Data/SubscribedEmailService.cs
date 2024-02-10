namespace HoneyWebPlatform.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HoneyWebPlatform.Data;
    using HoneyWebPlatform.Data.Models;
    using HoneyWebPlatform.Services.Data.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class SubscribedEmailService : ISubscribedEmailService
    {
        private readonly HoneyWebPlatformDbContext dbContext;

        public SubscribedEmailService(HoneyWebPlatformDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<SubscribedEmail>> GetSubscribedEmailsAsync()
        {
            return await this.dbContext
                .SubscribedEmails
                .ToListAsync();
        }

        public async Task AddSubscribedEmailAsync(string email)
        {
            // Check if the email is already subscribed
            if (await IsEmailSubscribedAsync(email))
            {
                return; // Do not add duplicate emails
            }

            // Add the email to the SubscribedEmails table
            await dbContext
                .SubscribedEmails
                .AddAsync(new SubscribedEmail { Email = email });

            await dbContext
                .SaveChangesAsync();
        }

        public async Task RemoveSubscribedEmailAsync(string email)
        {
            var subscribedEmail = await dbContext
                .SubscribedEmails
                .FirstOrDefaultAsync(se => se.Email == email);

            if (subscribedEmail != null)
            {
                dbContext.SubscribedEmails.Remove(subscribedEmail);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> IsEmailSubscribedAsync(string email)
        {
            return await dbContext
                .SubscribedEmails
                .AnyAsync(se => se.Email == email);
        }

    }
}