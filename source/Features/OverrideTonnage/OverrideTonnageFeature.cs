namespace MechEngineer.Features.OverrideTonnage
{
    internal class OverrideTonnageFeature : Feature
    {
        internal static readonly OverrideTonnageFeature Shared = new OverrideTonnageFeature();

        internal override bool Enabled => settings?.Enabled ?? false;

        internal static Settings settings => Control.settings.OverrideTonnage;

        public class Settings
        {
            public bool Enabled = true;
            public float FractionalAccountingPrecision = 0.5f; // change to 0.001 for kg fractional accounting precision
            public float? ArmorRoundingPrecision = null; // default is ARMOR_PER_STEP * TONNAGE_PER_ARMOR_POINT
        }
    }
}