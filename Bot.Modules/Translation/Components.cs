using DSharpPlus.EventArgs;
using LibreTranslate.Net;


namespace Bot.Modules.Translation
{
    public struct ReactionEvent
    {
        public MessageReactionAddEventArgs e;
    }

    public struct TranslationOptions
    {
        public LanguageCode? language;
        public string message;
    }

    public struct TranslatedMessage
    {
        public Task<string> translationTask;
    }
}