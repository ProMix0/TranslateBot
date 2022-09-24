using BicycleEcs;

namespace Bot.Modules.Translation
{
    public class TranslateSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly ITranslator translator;

        private IComponentPool<TranslationOptions> translationOptions = null!;
        private IComponentPool<TranslatedMessage> translates = null!;
        private IEcsFilter untranslated = null!;

        public TranslateSystem(ITranslator translator)
        {
            this.translator = translator;
        }

        public void Init(IEcsWorld world)
        {
            BicycleEcs.Utils.InjectPools(world.PoolsList, this);

            //translationOptions = world.PoolsList.GetComponentPool<TranslationOptions>();
            //translates = world.PoolsList.GetComponentPool<TranslatedMessage>();
            untranslated = world.FiltersManager.Filter().With<TranslationOptions>().Without<TranslatedMessage>().Build();
        }

        public void Run()
        {
            foreach (var entity in untranslated)
            {
                TranslationOptions options = translationOptions.GetComponent(entity);
                ref TranslatedMessage translated = ref translates.AddComponent(entity);
                translated.translationTask = translator.Translate(options.message, options.language!);
            }
        }
    }
}