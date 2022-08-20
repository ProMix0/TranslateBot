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
    public class TranslationModule : IBotModule
    {

        private readonly ITranslator translator;
        private readonly ITokenService tokenService;
        private readonly IMessageValidator validator;
        private readonly ILogger<TranslationModule> logger;

        public TranslationModule(ITranslator translator, ITokenService tokenService, IMessageValidator validator, ILogger<TranslationModule> logger)
        {
            this.translator = translator;
            this.tokenService = tokenService;
            this.validator = validator;
            this.logger = logger;
        }

        public void Register(DiscordClient client)
        {
            logger.LogDebug("Registering to {Event}", nameof(client.MessageReactionAdded));
            client.MessageReactionAdded += OnMessageReactionAdded;
        }

        public void Unregister(DiscordClient client)
        {
            logger.LogDebug("Unregistering from {Event}", nameof(client.MessageReactionAdded));
            client.MessageReactionAdded -= OnMessageReactionAdded;
        }

        private Task OnMessageReactionAdded(DiscordClient s, MessageReactionAddEventArgs e)
        {
            logger.LogDebug("Reaction received. Message id: {Id}, emoji: {Emoji}", e.Message.Id, e.Emoji.GetDiscordName());

            if (!e.Emoji.GetDiscordName().StartsWith(":flag_"))
                return Task.CompletedTask;

            logger.LogDebug("Reaction accepted. Begin translate");

            _ = Task.Run(async () =>
            {
                try
                {
                    DiscordMessage message = await e.Message.EnsureCached();

                    if (await validator.Validate(e, message))
                    {
                        await e.Channel.TriggerTypingAsync();

                        string translate = await translator.Translate(message.Content!, e.Emoji.GetDiscordName());

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
                }
                catch (Exception ex)
                {
                    ex.LogExceptionMessage(logger);
                }
            });
            return Task.CompletedTask;
        }
    }
}