namespace HoneyWebPlatform.Web.Infrastructure.Extensions
{
    using ViewModels.Flavour.Interfaces;
    using ViewModels.Category.Interfaces;

    public static class ViewModelsExtensions
    {
        public static string GetUrlInformation(this ICategoryDetailsModel model)
        {
            return model.Name.Replace(" ", "-");
        } 
        public static string GetUrlInformation(this IFlavourDetailsModel model)
        {
            return model.Name.Replace(" ", "-");
        }
    }
}
