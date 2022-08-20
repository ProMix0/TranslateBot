using DSharpPlus;
using DSharpPlus.Entities;

namespace Utils
{
    public static class DiscordUtils
    {
        public static async Task<DiscordMessage> EnsureCached(this DiscordMessage message)
        {
            if (message.Author == null)
                message = await message.Channel.GetMessageAsync(message.Id);

            return message;
        }
    }
}