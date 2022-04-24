namespace MechEngineer.Features.PlaceholderEffects;

public class PlaceholderEffectsSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Allows other features to support placeholder statistic effects.";

    public string ComponentEffectStatisticPrefix => "ComponentEffects";
    public char ComponentEffectStatisticSeparator => '-';
    public string ComponentEffectStatisticPlaceholder => "{uid}";
    public string ComponentEffectStatisticDescription => $"Components starting with {ComponentEffectStatisticPrefix}{ComponentEffectStatisticSeparator}{ComponentEffectStatisticPlaceholder}{ComponentEffectStatisticSeparator} will apply statistic effects to the components StatCollection instead of the mech. Really useful only for weapons.";
}