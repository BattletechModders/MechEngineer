namespace MechEngineer.Features.LocationalEffects
{
    public class LocationalEffectsSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Allows other features to support locational statistic effects.";
    }
}