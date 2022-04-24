using BattleTech;

namespace MechEngineer.Features.CriticalEffects;

public class CriticalEffectsSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Allows custom multiple critical hit states for individual components.";

    public bool DebugLogEffects { get; set; } = false;
    public string DebugLogEffectsDescription => "Log effect changes for debugging purposes.";

    public float DefaultMaxCritsPerSlots { get; set; } = 0.5f;
    public string DefaultMaxCritsPerSlotsDescription => "How many critical hits a component by default can take for each occupied slot. " +
                                                        "For CBT use 0, for Expanded Critical Damage behavior use 0.5. " +
                                                        "Custom CriticalEffects overwrite this behavior.";

    public ComponentType[] DefaultMaxCritsComponentTypes { get; set; } = {ComponentType.Weapon, ComponentType.AmmunitionBox, ComponentType.JumpJet, ComponentType.Upgrade};
    public string DefaultMaxCritsComponentTypesDescription => "For which types the default max crits are applied.";

    public string DescriptionIdentifier = "Criticals";
    public string DescriptionTemplate = "Critical Effects:<b><color=#F79B26FF>\r\n{{elements}}</color></b>\r\n";
    public string ElementTemplate = " <indent=10%><line-indent=-5%><line-height=65%>{{element}}</line-height></line-indent></indent>\r\n";
    public bool DescriptionUseName = false;

    public string CritFloatieMessage = "{0} CRIT";
    public string DestroyedFloatieMessage = "{0} DESTROYED";
    public string CritHitText = "HIT {0}: {1}";
    public string CritDestroyedText = "DESTROYED: {0}";
    public string CritDestroyedDeathText = "DESTROYED: Mech is incapacitated, reason is {0}";
    public string CritLinkedText = "Critical hits are linked to '{0}'";
}