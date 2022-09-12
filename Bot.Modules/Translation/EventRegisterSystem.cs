using System.Collections.Concurrent;
using BicycleEcs;
using DSharpPlus;
using DSharpPlus.EventArgs;
using LibreTranslate.Net;
using Microsoft.Extensions.Logging;
using Utils;

namespace Bot.Modules.Translation
{
    public class EventRegisterSystem : IEcsInitSystem, IEcsRunSystem
    {
        private ILogger<EventRegisterSystem> logger;

        private IComponentPool<ReactionEvent> reactions = null!;
        private IComponentPool<CachedMessage> cacheds = null!;
        private IEntitiesManager entities = null!;
        private readonly ConcurrentQueue<MessageReactionAddEventArgs> events = new();

        public EventRegisterSystem(ILogger<EventRegisterSystem> logger)
        {
            this.logger = logger;
        }

        public void RegisterEvent(MessageReactionAddEventArgs reaction)
        {
            events.Enqueue(reaction);
            logger.LogDebug("Message enqueued");
        }

        public void Init(IEcsWorld world)
        {
            reactions = world.PoolsList.GetComponentPool<ReactionEvent>();
            entities = world.EntitiesManager;
            cacheds = world.PoolsList.GetComponentPool<CachedMessage>();
        }

        public void Run()
        {
            while (events.TryDequeue(out MessageReactionAddEventArgs? reaction))
            {
                logger.LogDebug("Message dequeued");

                var entity = entities.CreateEntity();

                ref ReactionEvent reactionEvent = ref reactions.AddComponent(entity);
                reactionEvent.e = reaction;

                ref CachedMessage cached = ref cacheds.AddComponent(entity);
                cached.message = reaction.Message.EnsureCached().Result;
            }
        }

    }
}