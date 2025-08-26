namespace Common.Authorization;

public static class Permissions
{
    public static class Permission
    {
        public const string Read = "permissions:read";
        public const string Write = "permissions:write";
        public const string Delete = "permissions:delete";
        public const string Update = "permissions:update";
    }
    public static class Payment
    {
        public const string Read = "payment:read";
        public const string Write = "payment:write";
        public const string Delete = "payment:delete";
        public const string Update = "payment:update";
    }
    
}