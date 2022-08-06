using LibreTranslate.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslateBot.Common;

namespace TranslateBot
{
    internal class MirrorsLibreTranslator : ITranslator
    {
        private readonly LibreTranslate.Net.LibreTranslate[] translates;
        int index = 0;

        public MirrorsLibreTranslator(IOptions<MirrorsList> mirrors)
        {
            translates = mirrors.Value.Mirrors.Select(url => new LibreTranslate.Net.LibreTranslate(url)).ToArray();
        }

        public Task<string> Translate(string text, string emoji)
        {
            LanguageCode? to = FromEmoji(emoji);

            if (to == null)
                return Task.FromResult<string>(null);

            if (++index >= translates.Length) index = 0;

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

        public class MirrorsList
        {
            public const string SectionName = "LibreTranslate";

            public string[] Mirrors { get; set; } = Array.Empty<string>();
        }
    }
}
