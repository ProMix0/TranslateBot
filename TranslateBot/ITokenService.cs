using Microsoft.Extensions.Configuration;

namespace TranslateBot
{
    public interface ITokenService
    {
        string GetToken();
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetToken() => configuration["Token"];
    }
}