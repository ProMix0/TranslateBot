using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreTranslate.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot.Modules.Translation
{
    internal class MirrorsLibreTranslator : ITranslator
    {
        private readonly List<LibreTranslate.Net.LibreTranslate> translates = new();
        private readonly ILogger<MirrorsLibreTranslator> logger;
        int index = 0;

        public MirrorsLibreTranslator(IOptions<MirrorsList> mirrors, ILogger<MirrorsLibreTranslator> logger)
        {
            this.logger = logger;
            logger.LogDebug("Begin activity check for {Count} mirrors", mirrors.Value.Mirrors.Length);
            foreach (var url in mirrors.Value.Mirrors)
            {
                logger.LogTrace("Checking \"{Url}\" for activity", url);

                LibreTranslate.Net.LibreTranslate translate = new(url);
                if (IsActive(translate, url)) translates.Add(translate);
            }
            if (translates.Count == 0)
                logger.LogError("Activity check didn't found active mirrors");
            else
                logger.LogDebug("End activity check with {Count} active mirrors", translates.Count);
        }

        public Task<string> Translate(string text, string emoji)
        {
            LanguageCode? to = FromEmoji(emoji);

            if (to == null)
            {
                logger.LogDebug("Emoji \"{Emoji}\" not found. Returning null", emoji);
                return Task.FromResult<string>(string.Empty);
            }

            if (++index >= translates.Count) index = 0;

            logger.LogTrace("Translate using translate[{Index}]", index);

            logger.LogDebug("Source message: {Message}, target language: {Target}", text, to.ToString());

            return translates[index].TranslateAsync(new()
            {
                ApiKey = null,
                Source = LanguageCode.AutoDetect,
                Target = to,
                Text = text
            });
        }

        public static LanguageCode? FromEmoji(string emoji) => emoji switch
        {
            ":flag_sd:" => LanguageCode.Arabic,
            ":flag_cn:" => LanguageCode.Chinese,
            ":flag_us:" => LanguageCode.English,
            ":flag_um:" => LanguageCode.English,
            ":flag_gb:" => LanguageCode.English,
            ":flag_fr:" => LanguageCode.French,
            ":flag_de:" => LanguageCode.German,
            ":flag_in:" => LanguageCode.Hindi,
            ":flag_ie:" => LanguageCode.Irish,
            ":flag_it:" => LanguageCode.Italian,
            ":flag_jp:" => LanguageCode.Japanese,
            ":flag_kr:" => LanguageCode.Korean,
            ":flag_pt:" => LanguageCode.Portuguese,
            ":flag_ru:" => LanguageCode.Russian,
            ":flag_es:" => LanguageCode.Spanish,
            _ => null
        };

        private bool IsActive(LibreTranslate.Net.LibreTranslate translate, string url)
        {
            bool result = true;
            try
            {
                if (translate.TranslateAsync(new()
                {
                    Source = LanguageCode.AutoDetect,
                    Target = LanguageCode.Russian,
                    Text = "Test string"
                }).Result == null)
                {
                    logger.LogWarning("Translator from url \"{Url}\" return null", url);
                    result = false;
                }
            }
            catch
            {
                logger.LogWarning("Translator from url \"{Url}\" throws exception", url);
                result = false;
            }
            return result;
        }

        public class MirrorsList
        {
            public const string SectionName = "LibreTranslate";

            public string[] Mirrors { get; set; } = Array.Empty<string>();
        }
    }
}
