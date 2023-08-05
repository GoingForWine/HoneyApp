namespace HoneyWebPlatform.Services.Tests.DataForTests
{
    using System.Collections.Generic;
    using System.Linq;
    using HoneyWebPlatform.Data.Models;

    public static class Honeys
    {
        public static IEnumerable<Honey> TenPublicHoneys
            => Enumerable.Range(0, 10).Select(i => new Honey
            {
                Id = default,
                Title = null,
                Origin = null,
                Description = null,
                ImageUrl = null,
                Price = 6,
                NetWeight = 100,
                CreatedOn = default,
                YearMade = 0,
                IsActive = false,
            });
    }
}
