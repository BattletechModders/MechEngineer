namespace MechEngineer.Features.AccuracyEffects
{
    public class AccuracyEffectsSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Enables statistic effects for arm accuracy.";
    }
}