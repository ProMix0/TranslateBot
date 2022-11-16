using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Utils;

namespace Modules.Translation
{

    public class YandexTranslator : ITranslator
    {
        private string serviceKey;
        private readonly HttpClient client;
        private readonly string[] supportedLanguages;
        private readonly ILogger<YandexTranslator> logger;

        public YandexTranslator(IConfiguration configuration, ILogger<YandexTranslator> logger)
        {
            serviceKey = configuration.Find(logger,"SERVICE_TOKEN").GetTokenOrThrow();

            client = new();
            client.DefaultRequestHeaders.Add("Authorization", $"Api-Key {serviceKey}");

            this.logger = logger;

            supportedLanguages = GetSupportedLanguages();
        }


        public async Task<string> Translate(string text, string target)
        {
            using StringContent jsonContent = new StringContent($"{{\"targetLanguageCode\":\"{target}\",\"texts\":[\"{text}\"]}}",
             Encoding.UTF8, "application/json");

            logger.LogDebug("Request: {Request}", jsonContent.ReadAsStringAsync().Result);

            var response = await client.PostAsync("https://translate.api.cloud.yandex.net/translate/v2/translate", jsonContent);

            var jsonResponse = await response.Content.ReadAsStringAsync();

            logger.LogDebug("Response: {Response}", jsonResponse);

            var document = JsonDocument.Parse(jsonResponse);

            return document.RootElement.GetProperty("translations")[0].GetProperty("text").ToString();
        }

        public bool CanTranslateTo(string language) => supportedLanguages.Contains(language);

        private string[] GetSupportedLanguages()
        {
            using StringContent jsonContent = new StringContent("{}", Encoding.UTF8, "application/json");

            var response = client.PostAsync("https://translate.api.cloud.yandex.net/translate/v2/languages", jsonContent).Result;

            var jsonResponse = response.Content.ReadAsStringAsync().Result;

            var document = JsonDocument.Parse(jsonResponse);

            return document.RootElement.GetProperty("languages").EnumerateArray().Select(elem => elem.GetProperty("code").ToString()).ToArray();
        }
    }

}