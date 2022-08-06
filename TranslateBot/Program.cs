// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection;
using TranslateBot;
using TranslateBot.Common;
using Utils;

Console.WriteLine("Hello, World!");

IHost host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddUserSecrets(Assembly.GetAssembly(typeof(Program)));
    })
    .ConfigureServices((context, services) =>
    {
        services

        .AddOptions<MirrorsLibreTranslator.MirrorsList>(builder =>
            builder.BindConfiguration(MirrorsLibreTranslator.MirrorsList.SectionName))

        .AddTransient<ITranslator, MirrorsLibreTranslator>()

        .AddSingleton<ITokenService, TokenService>()

        .AddHostedService<BotService>();
    })
    .Build();

await host.RunAsync();