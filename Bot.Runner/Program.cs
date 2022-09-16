// See https://aka.ms/new-console-template for more information
using System.Reflection;
using Bot.Abstractions;
using Bot.Modules.Translation;
using Bot.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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

        .AddSingleton<ITokenService, TokenService>()

        .AddTranslation()

        .AddHostedService<ResolveInScope<BotService>>();
    })
    .Build();

await host.RunAsync();