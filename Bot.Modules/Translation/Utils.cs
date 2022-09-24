

namespace Bot.Modules.Translation
{
    public static class Utils
    {
        public static string ToLanguageCode(this string emoji) => emoji.Trim(':').Split('_').Last() switch
        {
            "us" => "en",
            "um" => "en",
            "gb" => "en",
            "cn" => "zh",
            "kr" => "ko",
            "jp" => "ja",
            string str => str
        };
    }
}