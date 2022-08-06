using System;
using System.Threading.Tasks;
using DSharpPlus;

namespace MyFirstBot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "MTAwMTU1ODgwNTE4MTYyODQ3Ng.Gp3NBG.qZYK6krQZXn1VIKU1OvQ8C-YktnKvqtdvU8yb4",
                TokenType = TokenType.Bot
            });

            discord.MessageCreated += async (s, e) =>
            {
                if (e.Message.Content.ToLower().StartsWith("ping"))
                    await e.Message.RespondAsync("pong!");

            };

            discord.MessageReactionAdded += async (s, e) =>
            {
                var message = e.Message;
                if (e.Message.Content == null)
                    message = await e.Channel.GetMessageAsync(e.Message.Id);
                await e.Message.RespondAsync($"Your message: {message.Content}");
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}