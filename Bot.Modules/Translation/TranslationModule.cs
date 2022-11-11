using BetterHostedServices;
using BicycleEcs;
using Bot.Abstractions;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Emzi0767.Utilities;
using LibreTranslate.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Utils;

namespace Bot.Modules.Translation
{
    public class TranslationModule : IBotModule
    {
        private readonly ILogger<TranslationModule> logger;

        private readonly IEcsContainer container;
        private readonly EventRegisterSystem register;
        private CancellationTokenSource? stopToken;

        public TranslationModule(EventRegisterSystem register, IServiceProvider provider, ILogger<TranslationModule> logger)
        {
            this.logger = logger;

            this.register = register;

            using IServiceScope scope = provider.CreateScope();

            IInjectContainerBuilder builder = new EcsContainerBuilder().UseSystemInjection(scope.ServiceProvider);
            container = builder
            .AddSystem(register)
            .AddSystem<FillTranslateOptionsSystem>()
            .AddSystem<ValidationSystem>()
            .AddSystem<TriggerTypingSystem>()
            .AddSystem<TranslateSystem>()
            .AddSystem<SendSystem>()
            .Build();

            container.Init();
        }

        public void Register(DiscordClient client)
        {
            logger.LogDebug("Registering to {Event}", nameof(client.MessageReactionAdded));
            client.MessageReactionAdded += OnMessageReactionAdded;

            stopToken = new();
            Task.Run(async () =>
            {
                try
                {
                    logger.LogDebug("Start Run() loop");
                    while (!stopToken.Token.IsCancellationRequested)
                    {
                        container.Run();
                        await Task.Delay(100);
                    }
                }
                catch (Exception e)
                {
                    e.LogExceptionMessage(logger);
                }
                logger.LogDebug("End Run() loop");
            }, stopToken.Token);
        }

        public void Unregister(DiscordClient client)
        {
            logger.LogDebug("Unregistering from {Event}", nameof(client.MessageReactionAdded));
            client.MessageReactionAdded -= OnMessageReactionAdded;
            stopToken?.Cancel();
        }

        private Task OnMessageReactionAdded(DiscordClient s, MessageReactionAddEventArgs e)
        {
            register.RegisterEvent(e);
            return Task.CompletedTask;
        }
    }
}