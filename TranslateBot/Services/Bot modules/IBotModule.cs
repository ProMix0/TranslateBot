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
    public interface IBotModule
    {
        void Register(DiscordClient client);
        void Unregister(DiscordClient client);
    }
}