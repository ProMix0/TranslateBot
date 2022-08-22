using DSharpPlus.EventArgs;
using LibreTranslate.Net;


namespace Bot.Modules.Translation
{
    public struct MessageToTranslate
    {
        public string message;
        public LanguageCode target;
        public MessageReactionAddEventArgs e;
    }
}