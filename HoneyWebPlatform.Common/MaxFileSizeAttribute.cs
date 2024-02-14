namespace HoneyWebPlatform.Common
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly long _maxFileSize;

        public MaxFileSizeAttribute(long maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult($"Файлът, който избрахте, е по-голям от позволеното. Изберете по-малък. ");
                }
            }

            return ValidationResult.Success;
        }
    }
}
