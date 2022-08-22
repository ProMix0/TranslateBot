using BicycleEcs;

namespace Bot.Modules.Translation
{
    public class TranslateSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly ITranslator translator;

        private IComponentPool<MessageToTranslate> messages = null!;
        private IComponentPool<TranslatedMessage> translates = null!;
        private IEcsFilter untranslated = null!;

        public TranslateSystem(ITranslator translator)
        {
            this.translator = translator;
        }

        public void Init(IEcsWorld world)
        {
            messages = world.PoolsList.GetComponentPool<MessageToTranslate>();
            translates = world.PoolsList.GetComponentPool<TranslatedMessage>();
            untranslated = world.FiltersManager.Filter().With<MessageToTranslate>().Without<TranslatedMessage>().Build();
        }

        public void Run()
        {
            try
            {
                foreach (var entity in untranslated)
                {
                    MessageToTranslate message = messages.GetComponent(entity);
                    ref TranslatedMessage translated = ref translates.AddComponent(entity);
                    translated.translationTask = translator.Translate(message.message, message.target);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}