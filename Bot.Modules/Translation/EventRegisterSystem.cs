using System.Collections.Concurrent;
using BicycleEcs;
using DSharpPlus;
using DSharpPlus.EventArgs;
using LibreTranslate.Net;
using Utils;

namespace Bot.Modules.Translation
{
    public class EventRegisterSystem : IEcsInitSystem, IEcsRunSystem
    {
        private IComponentPool<ReactionEvent> reactions = null!;
        private IEntitiesManager entities = null!;
        private readonly ConcurrentQueue<MessageReactionAddEventArgs> events = new();

        public void RegisterEvent(MessageReactionAddEventArgs reaction)
        {
            events.Enqueue(reaction);
        }

        public void Init(IEcsWorld world)
        {
            reactions = world.PoolsList.GetComponentPool<ReactionEvent>();
            entities = world.EntitiesManager;
        }

        public void Run()
        {
            while (events.TryDequeue(out MessageReactionAddEventArgs? reaction))
            {
                var entity = entities.CreateEntity();
                ref ReactionEvent reactionEvent = ref reactions.AddComponent(entity);
                reactionEvent.e = reaction;
            }
        }

    }
}