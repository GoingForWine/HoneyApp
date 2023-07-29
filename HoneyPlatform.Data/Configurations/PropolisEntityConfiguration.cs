namespace HoneyPlatform.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;

    public class PropolisEntityConfiguration : IEntityTypeConfiguration<Propolis>
    {
        public void Configure(EntityTypeBuilder<Propolis> builder)
        {
            builder
                .Property(h => h.CreatedOn)
                .HasDefaultValueSql("GETDATE()");

            builder
                .Property(h => h.IsActive)
                .HasDefaultValue(true);

            builder
                .HasOne(h => h.Flavour)
                .WithMany(c => c.Propolises)
                .HasForeignKey(h => h.FlavourId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(h => h.Beekeeper)
                .WithMany(a => a.OwnedPropolis)
                .HasForeignKey(h => h.BeekeeperId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(GeneratePropolises());
        }

        private Propolis[] GeneratePropolises()
        {
            ICollection<Propolis> propolises = new HashSet<Propolis>();

            Propolis propolis;

            //TODO seed appropriate honeys
            propolis = new Propolis()
            {
                Title = "Bee Propolis",
                Description = "30% tincture that is good for the immune system with anti-inflammatory properties.",
                ImageUrl = "https://www.apihealth.co.nz/wp-content/uploads/2019/07/Propolis-Tincture.jpg",
                Price = 3.00M,
                FlavourId = 1,
                //zzz
                BeekeeperId = Guid.Parse("F5086448-A741-4B80-B0C7-0263EB4E3AE9"), //BeekeeperId//UserId
            };
            propolises.Add(propolis);

            return propolises.ToArray();
        }
    }
}