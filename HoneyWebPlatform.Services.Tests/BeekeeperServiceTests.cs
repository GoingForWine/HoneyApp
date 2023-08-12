namespace HoneyWebPlatform.Services.Tests
{
    using Microsoft.EntityFrameworkCore;

    using Data;
    using Data.Interfaces;
    using HoneyWebPlatform.Data;

    using static DatabaseSeeder;

    public class BeekeeperServiceTests
    {
        // First way: Using InMemory Database
        // Pros: Testing is as close to the production scenario as possible
        // Cons: You are testing EFCore functionality as well, so this is not good UNIT test
        // Hard to arrange the scenario
        // Second way: Using Mock of IRepository
        // Pros: Good unit testing, tests single unit, easy push test data
        // Cons: You need to have repository pattern
        private DbContextOptions<HoneyWebPlatformDbContext> dbOptions;
        private HoneyWebPlatformDbContext dbContext;

        private IBeekeeperService beekeeperService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.dbOptions = new DbContextOptionsBuilder<HoneyWebPlatformDbContext>()
                .UseInMemoryDatabase("HoneyWebInMemory" + Guid.NewGuid().ToString())
                .Options;
            this.dbContext = new HoneyWebPlatformDbContext(this.dbOptions, false);

            this.dbContext.Database.EnsureCreated();
            SeedDatabase(this.dbContext);

            this.beekeeperService = new BeekeeperService(this.dbContext);

            var h = dbContext.Honeys.ToList();
        }

        [Test]
        public async Task BeekeeperExistsByUserIdAsyncShouldReturnTrueWhenExists()
        {
            string existingBeekeeperUserId = BeekeeperUser.Id.ToString();

            bool result = await this.beekeeperService.BeekeeperExistsByUserIdAsync(existingBeekeeperUserId);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task BeekeeperExistsByUserIdAsyncShouldReturnFalseWhenNotExists()
        {
            string existingUserId = User.Id.ToString();

            bool result = await this.beekeeperService.BeekeeperExistsByUserIdAsync(existingUserId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task BeekeeperExistsByPhoneNumberAsyncShouldReturnTrueWhenExists()
        {
            string existingBeekeeperNumber = BeekeeperUser.PhoneNumber.ToString();

            bool result = await this.beekeeperService.BeekeeperExistsByPhoneNumberAsync(existingBeekeeperNumber);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task BeekeeperExistsByPhoneNumberAsyncShouldReturnFalseWhenNotExists()
        {
            string existingUserNumber = User.PhoneNumber.ToString();

            bool result = await this.beekeeperService.BeekeeperExistsByPhoneNumberAsync(existingUserNumber);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task BeekeeperFullnameByHoneyIdAsyncShouldReturnCorrectNameWhenExists()
        {
            string existingHoneyId = dbContext.Honeys.First().Id.ToString();

            string result = await this.beekeeperService.BeekeeperFullnameByHoneyIdAsync(existingHoneyId);

            Assert.AreEqual("FirstName LastName", result);
        }


    }
}