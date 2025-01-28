using System.Security.Claims;

namespace Ingenious.Models.Account
{
    public class Principal
    {
        public string Name { get; set; }
        public List<Claim> Claims { get; set; }
    }
    public class AccessTokenModel
    {
        public string AccessToken { get; set; }
    }
    public class AccessRequestResponseDto : AccessTokenModel
    {
        public DateTime AccessTokeExpirationTime { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokeExpirationTime { get; set; }
    }
}
