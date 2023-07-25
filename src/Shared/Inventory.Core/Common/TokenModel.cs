namespace Inventory.Core.Common
{
    public class TokenModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
