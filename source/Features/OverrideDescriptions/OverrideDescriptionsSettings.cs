namespace MechEngineer.Features.OverrideDescriptions
{
    public class OverrideDescriptionsSettings : BaseSettings
    {
        public string BonusDescriptionsDescriptionTemplate = "Traits:<b><color=#F79B26FF>\r\n{{elements}}</color></b>\r\n{{originalDescription}}";
        public string BonusDescriptionsElementTemplate = " <indent=10%><line-indent=-5%><line-height=65%>{{element}}</line-height></line-indent></indent>\r\n";
    }
}