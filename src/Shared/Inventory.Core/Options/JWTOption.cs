namespace Inventory.Core.Options
{
    public class JWTOption
    {
        public const string JWTBearer = "JWTBearer";
        public string ValidAudience { get; set; } = string.Empty;
        public string ValidIssuer { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int ExpireTimeMinutes { get; set; }
    }
}
