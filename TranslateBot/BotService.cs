using BetterHostedServices;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Emzi0767.Utilities;
using LibreTranslate.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TranslateBot.Common;
using Utils;

namespace TranslateBot
{
    internal class BotService : NotEndingBackgroundService
    {
        private readonly ITranslator translator;
        private readonly ITokenService tokenService;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<BotService> logger;
        private readonly IMessageValidator validator;

        public BotService(ITranslator translator, ITokenService tokenService, IMessageValidator validator, ILoggerFactory loggerFactory, ILogger<BotService> logger)
        {
            this.translator = translator;
            this.tokenService = tokenService;
            this.validator = validator;
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

            client.MessageReactionAdded += OnMessageReactionAdded;

            await client.ConnectAsync();
        }

        private Task OnMessageReactionAdded(DiscordClient s, MessageReactionAddEventArgs e)
        {
            _ = Task.Run(async () =>
            {

                ValidationResult result = await validator.Validate(e);
                if (result.success)
                {
                    await e.Channel.TriggerTypingAsync();

                    string translate = await translator.Translate(result.result!, e.Emoji.GetDiscordName());

                    logger.LogDebug("Translated message: {Message}", translate);

                    if (string.IsNullOrEmpty(translate))
                    {
                        translate = "Unable to translate";
                        logger.LogDebug("Unable to translate message {Id} to language {Emoji}", e.Message.Id, e.Emoji.GetDiscordName());
                    }

                    await new DiscordMessageBuilder()
                    .WithContent($"{e.User.Mention}\n{translate}")
                    .WithAllowedMention(new UserMention(e.User))
                    .WithReply(e.Message.Id)
                    .SendAsync(e.Channel);
                }
            });
            return Task.CompletedTask;
        }

        protected override void OnError(Exception exceptionFromExecuteAsync)
        {
            exceptionFromExecuteAsync.LogExceptionMessage(logger);
        }
    }
}
