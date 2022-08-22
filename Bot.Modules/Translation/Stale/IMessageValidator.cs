using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bot.Modules.Translation
{
    public interface IMessageValidator
    {
        Task<bool> Validate(MessageReactionAddEventArgs e, DiscordMessage message);

    }

    public class MessageValidator : IMessageValidator
    {
        private readonly ILogger<MessageValidator> logger;

        public MessageValidator(ILogger<MessageValidator> logger)
        {
            this.logger = logger;
        }

        public Task<bool> Validate(MessageReactionAddEventArgs e, DiscordMessage message)
        {
            if (!e.Emoji.GetDiscordName().StartsWith(":flag_"))
            {
                logger.LogDebug("Fail validation reaction {Emoji} on message {Id} with incompatible emoji", e.Emoji.GetDiscordName(), e.Message.Id);
                return Task.FromResult(false);
            }

            if (message.Author.IsBot)
            {

                logger.LogDebug("Fail validation reaction {Emoji} on message {Id} with bot message", e.Emoji.GetDiscordName(), e.Message.Id);
                return Task.FromResult(false);
            }

            if (string.IsNullOrWhiteSpace(message.Content))
            {
                logger.LogDebug("Fail validation reaction {Emoji} on message {Id} with empty message", e.Emoji.GetDiscordName(), e.Message.Id);
                return Task.FromResult(false);
            }

            logger.LogDebug("Reaction {Emoji} on message {Id} was sucessful", e.Emoji.GetDiscordName(), e.Message.Id);
            return Task.FromResult(true);
        }
    }
}
