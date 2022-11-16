using Bot.Abstractions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Utils;

namespace Modules.Translation
{
    public class TranslationModule : IBotModule
    {
        private readonly ITranslator translator;
        private readonly ILogger<TranslationModule> logger;

        public TranslationModule(ITranslator translator, ILogger<TranslationModule> logger)
        {
            this.translator = translator;
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

        private async Task OnMessageReactionAdded(DiscordClient s, MessageReactionAddEventArgs e)
        {
            DiscordMessage message = await e.Message.EnsureCached();
            
            if (message.Author.IsBot)
                return;

            if (string.IsNullOrWhiteSpace(message.Content))
                return;
            
            logger.LogTrace("Message and author are valid");

            string language = e.Emoji.GetDiscordName().ToLanguageCode();
            logger.LogTrace("Language code: {Code}", language);
            if (!translator.CanTranslateTo(language))
                return;

            _ = TranslateAsync().ContinueWith(task => { task.Exception?.LogExceptionMessage(logger); },
                TaskContinuationOptions.NotOnRanToCompletion);

            async Task TranslateAsync()
            {
                await e.Channel.TriggerTypingAsync();

                string translation = await translator.Translate(message.Content, language);

                await new DiscordMessageBuilder()
                    .WithContent($"{e.User.Mention}\n{translation}")
                    .WithAllowedMention(new UserMention(e.User))
                    .WithReply(e.Message.Id)
                    .SendAsync(e.Channel);
            }
        }
    }
}