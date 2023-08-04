namespace HoneyWebPlatform.Common
{
    public static class EntityValidationConstants
    {
        public static class Category
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 50;
        }

        public static class Honey
        {
            public const int TitleMinLength = 10;
            public const int TitleMaxLength = 50;

            public const int OriginMinLength = 5;
            public const int OriginMaxLength = 150;

            public const int DescriptionMinLength = 30;
            public const int DescriptionMaxLength = 500;

            public const string PriceMinValue = "5";
            public const string PriceMaxValue = "25";


            public const int ImageUrlMaxLength = 2048;
        }

        public static class Beekeeper
        {
            public const int PhoneNumberMinLength = 7;
            public const int PhoneNumberMaxLength = 15;
        }

        public static class User
        {
            public const int PasswordMinLength = 6;
            public const int PasswordMaxLength = 100;

            public const int FirstNameMinLength = 1;
            public const int FirstNameMaxLength = 15;

            public const int LastNameMinLength = 1;
            public const int LastNameMaxLength = 15;
        }

        public static class Post
        {
            public const int TitleMaxLength = 50;
            public const int TitleMinLength = 10;

            public const int ContentMaxLength = 1500;
            public const int ContentMinLength = 30;
        }
    }
}