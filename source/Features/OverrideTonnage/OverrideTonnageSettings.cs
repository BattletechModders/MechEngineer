namespace MechEngineer.Features.OverrideTonnage
{
    public class OverrideTonnageSettings : BaseSettings
    {
        public float FractionalAccountingPrecision = 0.5f; // change to 0.001 for kg fractional accounting precision
        public float? ArmorRoundingPrecision = null; // default is ARMOR_PER_STEP * TONNAGE_PER_ARMOR_POINT
    }
}