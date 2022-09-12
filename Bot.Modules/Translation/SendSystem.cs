using BicycleEcs;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Utils;

namespace Bot.Modules.Translation
{
    public class SendSystem : IEcsRunSystem, IEcsInitSystem
    {
        private ILogger<SendSystem> logger;

        private IComponentPool<ReactionEvent> messages = null!;
        private IComponentPool<TranslatedMessage> translates = null!;
        private IEcsFilter translated = null!;
        private IEntitiesManager entities = null!;

        public SendSystem(ILogger<SendSystem> logger)
        {
            this.logger = logger;
        }

        public void Init(IEcsWorld world)
        {
            messages = world.PoolsList.GetComponentPool<ReactionEvent>();
            translates = world.PoolsList.GetComponentPool<TranslatedMessage>();
            translated = world.FiltersManager.Filter().With<ReactionEvent>().With<TranslatedMessage>().Build();
            entities = world.EntitiesManager;
        }

        public void Run()
        {
            foreach (var entity in translated)
            {
                ReactionEvent message = messages.GetComponent(entity);
                TranslatedMessage translate = translates.GetComponent(entity);

                if (!translate.translationTask.IsCompleted)
                {
                    logger.LogTrace("Translation not completed; skipping");
                    continue;
                }


                if (translate.translationTask.IsCompletedSuccessfully)
                    _ = new DiscordMessageBuilder()
                            .WithContent($"{message.e.User.Mention}\n{translate.translationTask.Result}")
                            .WithAllowedMention(new UserMention(message.e.User))
                            .WithReply(message.e.Message.Id)
                            .SendAsync(message.e.Channel).ContinueWith(task => task.Exception?.LogExceptionMessage(logger),
                             TaskContinuationOptions.NotOnRanToCompletion);
                else
                    translate.translationTask.Exception!.LogExceptionMessage(logger);

                entities.DeleteEntity(entity);
            }
        }
    }
}