﻿using BetterHostedServices;
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

        public BotService(ITranslator translator, ITokenService tokenService, ILoggerFactory loggerFactory, ILogger<BotService> logger)
        {
            this.translator = translator;
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

            client.MessageReactionAdded += OnMessageReactionAdded;

            await client.ConnectAsync();
        }

        private Task OnMessageReactionAdded(DiscordClient s, MessageReactionAddEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (!e.Emoji.GetDiscordName().StartsWith(":flag_"))
                    return;

                logger.LogTrace("Reaction received. Message id: {Id}, emoji: {Emoji}", e.Message.Id, e.Emoji.GetDiscordName());

                var message = e.Message;
                if (message.Content == null)
                    message = await e.Channel.GetMessageAsync(message.Id);

                string translate = await translator.Translate(message.Content, e.Emoji.GetDiscordName());

                if (string.IsNullOrEmpty(translate)) translate = "Unable to translate";

                await new DiscordMessageBuilder()
                .WithContent($"{e.User.Mention}:\n{translate}")
                .WithAllowedMention(new UserMention(e.User))
                .WithReply(e.Message.Id)
                .SendAsync(e.Channel);
            });
            return Task.CompletedTask;
        }

        protected override void OnError(Exception exceptionFromExecuteAsync)
        {
            exceptionFromExecuteAsync.LogExceptionMessage(logger);
        }
    }
}
