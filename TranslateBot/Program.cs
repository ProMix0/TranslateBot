// See https://aka.ms/new-console-template for more information
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TranslateBot;
using Utils;

Console.WriteLine("Hello, World!");

IHost host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        builder
        .AddUserSecrets(Assembly.GetAssembly(typeof(Program)))
        .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services

        .AddOptions<MirrorsLibreTranslator.MirrorsList>(builder =>
            builder.BindConfiguration(MirrorsLibreTranslator.MirrorsList.SectionName))

        .AddTransient<ITranslator, MirrorsLibreTranslator>()
        .AddTransient<IMessageValidator, MessageValidator>()

        .AddTransient<IBotModule, TranslationModule>()

        .AddSingleton<ITokenService, TokenService>()

        .AddHostedService<BotService>();
    })
    .Build();

await host.RunAsync();