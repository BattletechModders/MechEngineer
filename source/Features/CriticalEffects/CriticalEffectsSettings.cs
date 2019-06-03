namespace MechEngineer.Features.CriticalEffects
{
    public class CriticalEffectsSettings : BaseSettings
    {
        public string DescriptionTemplate = "Critical Effects:<b><color=#F79B26FF>\r\n{{elements}}</color></b>\r\n{{originalDescription}}";
        public string ElementTemplate = " <indent=10%><line-indent=-5%><line-height=65%>{{element}}</line-height></line-indent></indent>\r\n";
        public bool DescriptionUseName = false;

        public string CritFloatieMessage = "{0} CRIT";
        public string DestroyedFloatieMessage = "{0} DESTROYED";
    }
}