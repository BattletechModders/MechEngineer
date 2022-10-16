namespace MechEngineer.Features.ArmorStructureRatio;

public class ArmorStructureRatioSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "CBT rule enforcement, that armor at every mechs location is not more than 2 times the structure, head is allowed to be 3 times.";

    public string IgnoreChassisTag = "ignore_armor_structure_ratio";
    public string IgnoreChassisTagDescription => "(Obsolete) Skip a chassis when AutoFixing and mech validation. Not supported by ArmorMaximizer.";
}