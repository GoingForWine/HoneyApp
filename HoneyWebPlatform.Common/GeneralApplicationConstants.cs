namespace HoneyWebPlatform.Common
{
    public static class GeneralApplicationConstants
    {
        public const int ReleaseYear = 2023;

        public const int DefaultPage = 1;
        public const int EntitiesPerPage = 3;

        public const string AdminAreaName = "Admin";
        public const string AdminRoleName = "Administrator";
        public const string DevelopmentAdminEmail = "admin@honeyplatform.bg";

        public const string UsersCacheKey = "UsersCache";
        public const int UsersCacheDurationMinutes = 5;

        public const string OnlineUsersCookieName = "IsOnline";
        public const int LastActivityBeforeOfflineMinutes = 10;
    }
}