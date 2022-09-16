using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using LibreTranslate.Net;


namespace Bot.Modules.Translation
{
    public struct ReactionEvent
    {
        public MessageReactionAddEventArgs e;
    }

    public struct CachedMessage
    {
        public Task<DiscordMessage> message;
    }

    public struct TranslationOptions
    {
        public string language;
        public string message;
    }

    public struct TranslatedMessage
    {
        public Task<string> translationTask;
    }

    public struct TypingTriggered { }
}