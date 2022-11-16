using Bot.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Utils;

namespace Modules.Translation
{
    public static class DI
    {
        public static IServiceCollection AddTranslation(this IServiceCollection services) =>
            services
                .AddSingleton<ITranslator, YandexTranslator>()
                .AddTransient<IBotModule, TranslationModule>();

    }
}