using Microsoft.Extensions.Configuration;

namespace TranslateBot
{
    public interface ITokenService
    {
        string Token { get; }
    }

    public class TokenService : ITokenService
    {
        public string Token { get; }

        public TokenService(IConfiguration configuration)
        {
            Token = configuration["Token"];
        }
    }
}