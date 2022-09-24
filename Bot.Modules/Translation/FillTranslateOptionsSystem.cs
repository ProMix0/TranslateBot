using System.Collections.Concurrent;
using BicycleEcs;
using DSharpPlus;
using DSharpPlus.EventArgs;
using LibreTranslate.Net;
using Microsoft.Extensions.Logging;
using Utils;

namespace Bot.Modules.Translation
{
    public class FillTranslateOptionsSystem : IEcsRunSystem, IEcsInitSystem
    {
        private ILogger<FillTranslateOptionsSystem> logger;

        private IComponentPool<ReactionEvent> reactions = null!;
        private IComponentPool<TranslationOptions> translationOptions = null!;
        private IComponentPool<CachedMessage> cacheds = null!;

        private IEcsFilter filter = null!;

        public FillTranslateOptionsSystem(ILogger<FillTranslateOptionsSystem> logger)
        {
            this.logger = logger;
        }

        public void Init(IEcsWorld world)
        {
            BicycleEcs.Utils.InjectPools(world.PoolsList, this);

            //reactions = world.PoolsList.GetComponentPool<ReactionEvent>();
            //translationOptions = world.PoolsList.GetComponentPool<TranslationOptions>();
            //cacheds = world.PoolsList.GetComponentPool<CachedMessage>();
            filter = world.FiltersManager.Filter().With<ReactionEvent>().With<CachedMessage>().Without<TranslationOptions>().Build();
        }

        public void Run()
        {
            foreach (var entity in filter)
            {
                ReactionEvent reaction = reactions.GetComponent(entity);
                CachedMessage cached = cacheds.GetComponent(entity);

                if (!cached.message.IsCompleted)
                {
                    logger.LogTrace($"Skip adding {nameof(TranslationOptions)} due to incompleted cache task");
                    continue;
                }

                ref TranslationOptions translation = ref translationOptions.AddComponent(entity);
                translation.message = cached.message.Result.Content;
                translation.language = reaction.e.Emoji.GetDiscordName().ToLanguageCode();
            }
        }
    }
}