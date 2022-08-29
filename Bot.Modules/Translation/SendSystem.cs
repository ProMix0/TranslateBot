using BicycleEcs;
using DSharpPlus;
using DSharpPlus.Entities;

namespace Bot.Modules.Translation
{
    public class SendSystem : IEcsRunSystem, IEcsInitSystem
    {

        private IComponentPool<ReactionEvent> messages = null!;
        private IComponentPool<TranslatedMessage> translates = null!;
        private IEcsFilter translated = null!;
        private IEntitiesManager entities = null!;


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

                if (!translate.translationTask.IsCompleted) continue;

                _ = new DiscordMessageBuilder()
                        .WithContent($"{message.e.User.Mention}\n{translate.translationTask.Result}")
                        .WithAllowedMention(new UserMention(message.e.User))
                        .WithReply(message.e.Message.Id)
                        .SendAsync(message.e.Channel);

                entities.DeleteEntity(entity);
            }
        }
    }
}