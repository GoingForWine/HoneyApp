namespace HoneyWebPlatform.Web.ViewModels.Blog
{
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;

    using Data.Models;
    using Services.Mapping;

    using static Common.EntityValidationConstants.Post;

    public class CommentFormModel : IMapTo<Comment>, IHaveCustomMappings
    {
        [StringLength(ContentMaxLength, MinimumLength = ContentMinLength,
            ErrorMessage = "Съдържанието трябва да е с дължина между {2} и {1} символа.")]
        [Required(ErrorMessage = "Моля, въведете съдържанието.")]
        public string Content { get; set; } = null!;
      
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<CommentFormModel, Comment>()
                .ForMember(d => d.AuthorId, opt => opt.Ignore())
                .ForMember(d => d.IsActive, opt => opt.UseDestinationValue()); // If IsActive is not in the form model
            //todo careful if this last line bugs with our active state when changing/creating entities.
        }
    }
}