namespace Modules.Translation
{
    public interface ITranslator
    {
        public Task<string> Translate(string text, string target);

        public bool CanTranslateTo(string language);
    }
}
