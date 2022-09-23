using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bot.Abstractions
{
    public interface ITokenService
    {
        TokenResult Find(params string[] keys);
    }

    public struct TokenResult
    {
        internal TokenResult(string token, bool success)
        {
            Token = token;
            IsSuccessful = success;
        }

        public readonly string Token;
        public readonly bool IsSuccessful;

        internal static TokenResult Success(string token) => new(token, true);
        internal static TokenResult Fail() => new(null!, false);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<TokenService> logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public TokenResult Find(params string[] keys)
        {
            string? token = null;

            foreach (var key in keys)
                if (TrySetToken(key, ref token!))
                    break;

            if (token == null)
            {
                logger.LogCritical("Can't get token from keys");
                return TokenResult.Fail();
            }
            return TokenResult.Success(token);
        }

        private bool TrySetToken(string key, ref string token)
        {
            string tempToken = configuration[key];
            if (tempToken != null)
            {
                logger.LogDebug("Get token from {Key}", key);
                token = tempToken;
                return true;
            }
            return false;
        }
    }
}