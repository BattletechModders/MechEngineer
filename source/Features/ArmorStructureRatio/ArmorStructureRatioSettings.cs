namespace MechEngineer.Features.ArmorStructureRatio
{
    public class ArmorStructureRatioSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Enforces CBT armor to structure ratios for all compartments of a mech.";

        public string[] SkipMechDefs = { };
    }
}