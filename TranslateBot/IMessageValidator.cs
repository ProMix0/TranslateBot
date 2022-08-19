using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TranslateBot
{
    public interface IMessageValidator
    {
        Task<ValidationResult> Validate(MessageReactionAddEventArgs e);

    }

    public struct ValidationResult
    {

        public static ValidationResult Success(string result)
        {
            return new(true, result);
        }

        public static ValidationResult Fail()
        {
            return new(false, null);
        }

        private ValidationResult(bool success, string? result)
        {
            this.success = success;
            this.result = result;
        }

        public readonly bool success;
        public readonly string? result;
    }

    public class MessageValidator : IMessageValidator
    {
        private readonly ILogger<MessageValidator> logger;

        public MessageValidator(ILogger<MessageValidator> logger)
        {
            this.logger = logger;
        }

        public async Task<ValidationResult> Validate(MessageReactionAddEventArgs e)
        {
            if (!e.Emoji.GetDiscordName().StartsWith(":flag_"))
            {
                logger.LogDebug("Fail validation reaction {Emoji} on message {Id} with incompatible emoji", e.Emoji.GetDiscordName(), e.Message.Id);
                return ValidationResult.Fail();
            }

            string message = e.Message.Content;
            if (message == null)
                message = (await e.Channel.GetMessageAsync(e.Message.Id)).Content;

            if (string.IsNullOrWhiteSpace(message))
            {
                logger.LogDebug("Fail validation reaction {Emoji} on message {Id} with empty message", e.Emoji.GetDiscordName(), e.Message.Id);
                return ValidationResult.Fail();
            }

            logger.LogDebug("Reaction {Emoji} on message {Id} was sucessful", e.Emoji.GetDiscordName(), e.Message.Id);
            return ValidationResult.Success(message);
        }
    }
}
