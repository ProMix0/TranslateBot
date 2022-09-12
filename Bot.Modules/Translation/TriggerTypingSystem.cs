using BicycleEcs;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Utils;

namespace Bot.Modules.Translation
{
    public class TriggerTypingSystem : IEcsRunSystem, IEcsInitSystem
    {
        private ILogger<TriggerTypingSystem> logger;

        private IComponentPool<ReactionEvent> messages = null!;
        private IComponentPool<TypingTriggered> typings = null!;
        private IEcsFilter notTriggered = null!;

        public TriggerTypingSystem(ILogger<TriggerTypingSystem> logger)
        {
            this.logger = logger;
        }

        public void Init(IEcsWorld world)
        {
            messages = world.PoolsList.GetComponentPool<ReactionEvent>();
            typings = world.PoolsList.GetComponentPool<TypingTriggered>();
            notTriggered = world.FiltersManager.Filter().With<ReactionEvent>().With<TranslatedMessage>().Without<TypingTriggered>().Build();
        }

        public void Run()
        {
            foreach (var entity in notTriggered)
            {
                ReactionEvent message = messages.GetComponent(entity);

                message.e.Channel.TriggerTypingAsync();
                typings.AddComponent(entity);
            }
        }
    }
}