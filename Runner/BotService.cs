using BetterHostedServices;
using Bot.Abstractions;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Emzi0767.Utilities;
using LibreTranslate.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Utils;

namespace Bot.Runner
{
    internal class BotService : NotEndingBackgroundService
    {
        private readonly IEnumerable<IBotModule> modules;
        private readonly IConfiguration configuration;
        private readonly ILogger<BotService> logger;
        private readonly ILoggerFactory loggerFactory;

        public BotService(IEnumerable<IBotModule> modules, IConfiguration configuration, ILoggerFactory loggerFactory,
            ILogger<BotService> logger)
        {
            this.modules = modules;
            this.configuration = configuration;
            this.loggerFactory = loggerFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            DiscordClient client = new(new DiscordConfiguration
            {
                LoggerFactory = loggerFactory,
                Token = configuration.Find(logger, "DISCORD_TOKEN").GetTokenOrThrow(),
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