namespace Inventory.Core.Configurations
{
    public class JwtConfig
    {
        private const string _name = "Bearer";
        public static string Name => _name;
        public string Audience { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int ExpireMinutes { get; set; }
    }
}
