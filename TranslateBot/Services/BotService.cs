using BetterHostedServices;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Emzi0767.Utilities;
using LibreTranslate.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Utils;

namespace TranslateBot
{
    internal class BotService : NotEndingBackgroundService
    {
        private readonly IEnumerable<IBotModule> modules;
        private readonly ITokenService tokenService;
        private readonly ILogger<BotService> logger;
        private readonly ILoggerFactory loggerFactory;

        public BotService(IEnumerable<IBotModule> modules, ITokenService tokenService, ILoggerFactory loggerFactory, ILogger<BotService> logger)
        {
            this.modules = modules;
            this.tokenService = tokenService;
            this.loggerFactory = loggerFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DiscordClient client = new(new()
            {
                LoggerFactory = loggerFactory,
                Token = tokenService.Token,
                TokenType = TokenType.Bot
            });

            foreach (var module in modules)
                module.Register(client);

            await client.ConnectAsync();
        }

        protected override void OnError(Exception exceptionFromExecuteAsync)
        {
            exceptionFromExecuteAsync.LogExceptionMessage(logger);
        }
    }
}
