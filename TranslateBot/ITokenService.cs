using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TranslateBot
{
    public interface ITokenService
    {
        string Token { get; }
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<TokenService> logger;

        private readonly string[] keys = { "Token", "DISCORD_TOKEN" };

        public string Token { get; private set; }

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;

            foreach (var key in keys)
                if (TrySetToken(key))
                    break;

            if (Token == null)
                logger.LogCritical("Can't get token from keys");
        }

        private bool TrySetToken(string key)
        {
            Token = configuration[key];
            if (Token != null)
            {
                logger.LogDebug("Get token from {Key}", key);
                return true;
            }
            return false;
        }
    }
}