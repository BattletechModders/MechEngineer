namespace MechEngineer.Features.CriticalEffects
{
    public class CriticalEffectsSettings : BaseSettings
    {
        public string DescriptionTemplate = "Critical Effects:<b><color=#F79B26FF>\r\n{{elements}}</color></b>\r\n{{originalDescription}}";
        public bool DescriptionUseName = false;
    }
}