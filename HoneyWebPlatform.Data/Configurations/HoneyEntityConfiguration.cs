namespace HoneyWebPlatform.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;

    public class HoneyEntityConfiguration : IEntityTypeConfiguration<Honey>
    {
        public void Configure(EntityTypeBuilder<Honey> builder)
        {
            builder
                .Property(h => h.CreatedOn)
                .HasDefaultValueSql("GETDATE()");

            builder
                .Property(h => h.IsActive)
                .HasDefaultValue(true);

            builder
                .HasOne(h => h.Category)
                .WithMany(c => c.Honeys)
                .HasForeignKey(h => h.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(h => h.Beekeeper)
                .WithMany(a => a.OwnedHoney)
                .HasForeignKey(h => h.BeekeeperId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(GenerateHoneys());
        }

        private Honey[] GenerateHoneys()
        {
            ICollection<Honey> honeys = new HashSet<Honey>();

            Honey honey;

            honey = new Honey()
            {
                Title = "Asen's Linden Honey",
                Origin = "Vratsa",
                Description = "High quality linden honey from Asen Asenev.",
                ImageUrl = "https://beehoneyportal.com/wp-content/uploads/2014/10/burkan-s-med-3.jpg",
                Price = 12.00M,
                NetWeight = 450,
                YearMade = 2022,
                CategoryId = 1,
                BeekeeperId = Guid.Parse("A38F5E91-11FF-40B2-9987-317C60FEC5A5"), //BeekeeperId//UserId
            };
            honeys.Add(honey);

            return honeys.ToArray();
        }
    }
}
