namespace HoneyWebPlatform.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;

    public class FlavourEntityConfiguration : IEntityTypeConfiguration<Flavour>
    {
        public void Configure(EntityTypeBuilder<Flavour> builder)
        {
            //builder.HasData(GenerateFlavours());
        }

        private Flavour[] GenerateFlavours()
        {
            ICollection<Flavour> flavours = new HashSet<Flavour>();

            Flavour flavour;

            flavour = new Flavour()
            {
                Id = 1,
                Name = "Natural"
            };
            flavours.Add(flavour);

            flavour = new Flavour()
            {
                Id = 2,
                Name = "Strawberry"
            };
            flavours.Add(flavour);

            flavour = new Flavour()
            {
                Id = 3,
                Name = "Mint and Ginger"
            };
            flavours.Add(flavour);

            return flavours.ToArray();
        }
    }
}