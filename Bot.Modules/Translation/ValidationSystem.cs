using System.Collections.Concurrent;
using BicycleEcs;
using DSharpPlus;
using DSharpPlus.EventArgs;
using LibreTranslate.Net;
using Utils;

namespace Bot.Modules.Translation
{
    public class ValidationSystem : IEcsInitSystem, IEcsRunSystem
    {
        private IComponentPool<TranslationOptions> translationOptions = null!;
        private IComponentPool<ReactionEvent> events = null!;
        private IComponentPool<CachedMessage> cacheds = null!;
        private IEntitiesManager entities = null!;
        private IEcsFilter filter = null!;

        private ITranslator translator;

        public ValidationSystem(ITranslator translator)
        {
            this.translator = translator;
        }

        public void Init(IEcsWorld world)
        {
            translationOptions = world.PoolsList.GetComponentPool<TranslationOptions>();
            events = world.PoolsList.GetComponentPool<ReactionEvent>();
            cacheds = world.PoolsList.GetComponentPool<CachedMessage>();
            entities = world.EntitiesManager;
            filter = world.FiltersManager.Filter().With<TranslationOptions>().Without<TranslatedMessage>().Build();
        }

        public void Run()
        {
            foreach (var entity in filter)
            {
                ReactionEvent reactionEvent = events.GetComponent(entity);
                TranslationOptions options = translationOptions.GetComponent(entity);
                CachedMessage cached = cacheds.GetComponent(entity);

                bool valid = true;

                if (options.language == null)
                    valid = false;


                if (cached.message.Result.Author.IsBot)
                    valid = false;

                if (!translator.CanTranslateTo(options.language!))
                    valid = false;

                if (string.IsNullOrWhiteSpace(options.message))
                    valid = false;

                if (!valid)
                    entities.DeleteEntity(entity);

            }
        }

    }
}