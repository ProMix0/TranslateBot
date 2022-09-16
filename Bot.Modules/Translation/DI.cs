using System.Reflection;
using Bot.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Utils;

namespace Bot.Modules.Translation
{
    public static class DI
    {
        public static IServiceCollection AddTranslation(this IServiceCollection services) =>
            services
                //.AddTransient<IMessageValidator, MessageValidator>()
                .AddScoped<ITranslator, YandexTranslator>()
                .AddTransient<IBotModule, TranslationModule>()
                .AddOptions<MirrorsLibreTranslator.MirrorsList>(builder =>
                    builder.BindConfiguration(MirrorsLibreTranslator.MirrorsList.SectionName))

                .AddTransient<FillTranslateOptionsSystem>()
                .AddTransient<ValidationSystem>()
                .AddTransient<TranslateSystem>()
                .AddTransient<SendSystem>()
                .AddTransient<EventRegisterSystem>()
                .AddTransient<TriggerTypingSystem>();

    }
}