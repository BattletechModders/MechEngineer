namespace MechEngineer.Features.OverrideDescriptions;

public class OverrideDescriptionsSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Allows other features to override tooltips and descriptions, also provides bonus description management.";

    public string DescriptionIdentifier = "Bonuses";
    public string BonusDescriptionsDescriptionTemplate = "Traits:<b><color=#F79B26FF>\r\n{{elements}}</color></b>\r\n";
    public string BonusDescriptionsElementTemplate = " <indent=10%><line-indent=-5%><line-height=65%>{{element}}</line-height></line-indent></indent>\r\n";
}