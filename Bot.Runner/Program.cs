// See https://aka.ms/new-console-template for more information
using System.Reflection;
using Bot.Abstractions;
using Bot.Modules.Translation;
using Bot.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sentry;
using Sentry.Extensions;
using Sentry.Extensions.Logging;
using Sentry.Integrations;
using Utils;

Console.WriteLine("Hello, World!");

IHost host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        builder
        .AddUserSecrets(Assembly.GetAssembly(typeof(Program)))
        .AddEnvironmentVariables();
    })
    .ConfigureLogging((context, builder) =>
    {
        builder

        .AddConfiguration(context.Configuration)
        .AddSentry();
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