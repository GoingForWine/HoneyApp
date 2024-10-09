namespace HoneyWebPlatform.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;

    public class CategoryEntityConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            //builder.HasData(GenerateCategories());
        }

        private Category[] GenerateCategories()
        {
            ICollection<Category> categories = new HashSet<Category>();

            Category category;

            category = new Category()
            {
                Id = 1,
                Name = "Linden"
            };
            categories.Add(category);

            category = new Category()
            {
                Id = 2,
                Name = "Bio"
            };
            categories.Add(category);

            category = new Category()
            {
                Id = 3,
                Name = "Sunflower"
            };
            categories.Add(category);

            category = new Category()
            {
                Id = 4,
                Name = "Bouquet"
            };
            categories.Add(category);

            category = new Category()
            {
                Id = 5,
                Name = "Honeydew"
            };
            categories.Add(category);

            return categories.ToArray();
        }
    }
}