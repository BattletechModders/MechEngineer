namespace MechEngineer.Features.CriticalEffects
{
    public class CriticalEffectsSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Allows custom multiple critical hit states for individual components.";

        public string DescriptionTemplate = "Critical Effects:<b><color=#F79B26FF>\r\n{{elements}}</color></b>\r\n{{originalDescription}}";
        public string ElementTemplate = " <indent=10%><line-indent=-5%><line-height=65%>{{element}}</line-height></line-indent></indent>\r\n";
        public bool DescriptionUseName = false;

        public string CritFloatieMessage = "{0} CRIT";
        public string DestroyedFloatieMessage = "{0} DESTROYED";
        public string CritHitText = "HIT {0}: {1}";
        public string CritDestroyedText = "DESTROYED: {0}";
        public string CritDestroyedDeathText = "DESTROYED: Mech is incapacitated, reason is {0}";
        public string CritLinkedText = "Critical hits are linked to '{0}'";
    }
}