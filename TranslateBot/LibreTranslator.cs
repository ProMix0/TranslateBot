using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslateBot.Common;
using LibreTranslate.Net;

namespace TranslateBot
{
    internal class LibreTranslator : ITranslator
    {
        LibreTranslate.Net.LibreTranslate translate;

        public LibreTranslator()
        {
            translate = new("https://libretranslate.de/");


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

        public Task<string> Translate(string text, string emoji)
        {
            LanguageCode? to = FromEmoji(emoji);

            if (to == null)
                return Task.FromResult<string>(null);

            return translate.TranslateAsync(new()
            {
                ApiKey = null,
                Source = LanguageCode.AutoDetect,
                Target = to,
                Text = text
            });
        }
    }
}
