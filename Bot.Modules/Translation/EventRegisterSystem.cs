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
        private IComponentPool<MessageToTranslate> messages = null!;
        private IEntitiesManager entities = null!;
        private readonly ConcurrentQueue<MessageReactionAddEventArgs> events = new();

        public void RegisterEvent(MessageReactionAddEventArgs reaction)
        {
            events.Enqueue(reaction);
        }

        public void Init(IEcsWorld world)
        {
            messages = world.PoolsList.GetComponentPool<MessageToTranslate>();
            entities = world.EntitiesManager;
        }

        public void Run()
        {
            while (events.TryDequeue(out MessageReactionAddEventArgs? reaction))
            {
                var entity = entities.CreateEntity();
                ref MessageToTranslate message = ref messages.AddComponent(entity);
                message.message = reaction.Message.EnsureCached().Result.Content;
                message.target = MirrorsLibreTranslator.FromEmoji(reaction.Emoji.GetDiscordName())!;
                message.e = reaction;
            }
        }

    }
}