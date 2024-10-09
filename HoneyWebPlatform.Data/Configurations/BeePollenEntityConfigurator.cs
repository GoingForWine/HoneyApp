namespace HoneyWebPlatform.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;

    public class BeePollenEntityConfigurator : IEntityTypeConfiguration<BeePollen>
    {
        public void Configure(EntityTypeBuilder<BeePollen> builder)
        {
            builder
                .Property(h => h.CreatedOn)
                .HasDefaultValueSql("GETDATE()");

            builder
                .Property(h => h.IsActive)
                .HasDefaultValue(true);

            builder
                .HasOne(h => h.Beekeeper)
                .WithMany(a => a.OwnedBeePollen)
                .HasForeignKey(h => h.BeekeeperId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.HasData(GenerateBeePollens());
        }

        private BeePollen[] GenerateBeePollens()
        {
            ICollection<BeePollen> beePollens = new HashSet<BeePollen>();

            BeePollen beePollen;

            beePollen = new BeePollen()
            {
                Title = "Asen's Bee Pollen",
                Description = "Asen Asenev's rich in vitamins and minerals bee pollen. It has the riches of bulgarian nature",
                ImageUrl = "https://naturalvita.co.uk/wp-content/uploads/2018/03/bee-pollen-natruralvita-1024x1024.jpg",
                Price = 9.00M,
                NetWeight = 100,
                BeekeeperId = Guid.Parse("7ADAF90E-FEC8-492E-8760-FE3190F1D689"), 
                CreatedOn = DateTime.UtcNow,
                IsActive = true
            };
            beePollens.Add(beePollen);

            return beePollens.ToArray();
        }
    }
}



