namespace HoneyWebPlatform.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;

    public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder
                .Property(u => u.FirstName)
                .HasDefaultValue("Test");

            builder
                .Property(u => u.LastName)
                .HasDefaultValue("Testov");

            // Add configurations for ApplicationUser properties
            builder
                .Property(u => u.ProfilePicturePath)
                .HasMaxLength(255)
                .IsRequired(false); // Depending on whether it's mandatory
        }
    }
}