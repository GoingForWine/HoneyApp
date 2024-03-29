﻿namespace HoneyWebPlatform.Web.ViewModels.Blog
{
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    
    using Data.Models;
    using Common;
    using Microsoft.AspNetCore.Http;
    using Services.Mapping;
    
    using static Common.EntityValidationConstants.Post;

    public class PostFormModel : IMapTo<Post>, IHaveCustomMappings
    {
        [Required(ErrorMessage = "Моля, въведете заглавието.")]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength,
            ErrorMessage = "Заглавието трябва да е с дължина между {2} и {1} символа.")]
        public string Title { get; set; } = null!;

        [StringLength(ContentMaxLength, MinimumLength = ContentMinLength,
            ErrorMessage = "Съдържанието трябва да е с дължина между {2} и {1} символа.")]
        [Required(ErrorMessage = "Моля, въведете съдържанието.")]
        public string Content { get; set; } = null!;

        [Required(ErrorMessage = "Моля добавете снимка до 2MB.")]
        [Display(Name = "Снимка")]
        [MaxFileSize(ProfilePictureMaxSize, ErrorMessage = "Максималният размер на файла за снимка е 2 мегабайта.")]
        public IFormFile? PostPicture { get; set; }

        public string PostPicturePath { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<PostFormModel, Post>()
                .ForMember(d => d.AuthorId, opt => opt.Ignore())
                .ForMember(d => d.IsActive, opt => opt.UseDestinationValue()); // If IsActive is not in the form model
                                                                               //todo careful if this last line bugs with our active state when changing/creating entities.
        }
    }
}
