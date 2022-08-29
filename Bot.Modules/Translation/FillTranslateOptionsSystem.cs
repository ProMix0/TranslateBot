using System.Collections.Concurrent;
using BicycleEcs;
using DSharpPlus;
using DSharpPlus.EventArgs;
using LibreTranslate.Net;
using Utils;

namespace Bot.Modules.Translation
{
    public class FillTranslateOptionsSystem : IEcsRunSystem, IEcsInitSystem
    {
        private IComponentPool<ReactionEvent> reactions = null!;
        private IComponentPool<TranslationOptions> translationOptions = null!;

        private IEcsFilter filter = null!;

        public void Init(IEcsWorld world)
        {
            reactions = world.PoolsList.GetComponentPool<ReactionEvent>();
            translationOptions = world.PoolsList.GetComponentPool<TranslationOptions>();
            filter = world.FiltersManager.Filter().With<ReactionEvent>().Without<TranslationOptions>().Build();
        }

        public void Run()
        {
            foreach (var entity in filter)
            {
                ReactionEvent reaction = reactions.GetComponent(entity);
                ref TranslationOptions translation = ref translationOptions.AddComponent(entity);
                translation.message = reaction.e.Message.EnsureCached().Result.Content;
                translation.language = MirrorsLibreTranslator.FromEmoji(reaction.e.Emoji.GetDiscordName());
            }
        }
    }
}