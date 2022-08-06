// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using TranslateBot;
using TranslateBot.Common;

Console.WriteLine("Hello, World!");

IHost host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddUserSecrets(Assembly.GetAssembly(typeof(Program)));
    })
    .ConfigureServices(services =>
    {
        services

        .AddTransient<ITranslator, LibreTranslator>()

        .AddHostedService<BotService>();
    })
    .Build();

await host.RunAsync();