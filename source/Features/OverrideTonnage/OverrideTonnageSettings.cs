namespace MechEngineer.Features.OverrideTonnage;

public class OverrideTonnageSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Allows other features to override tonnage calculations.";

    public float PrecisionEpsilon = 0.0001f;
    public string PrecisionEpsilonDescription = "The maximum tonnage two values can be apart to be viewed as being the same, vanilla uses 100 grams.";

    public float TonnageStandardPrecision = 0.5f;
    public string TonnageStandardPrecisionDescription = "Set to 0.001 for kg fractional accounting on BattleMechs, only modifies half-ton rounding operations as by rule";

    public float KilogramStandardPrecision = 0.001f;
    public string KilogramStandardPrecisionDescription = "Used for Small Support Vehicles, ProtoMechs and battlesuits.";

    public float ArmorRoundingPrecision = 0.001f;
    public string ArmorRoundingPrecisionDescription = "CBT standard rules say this should be 0.5, but that would round off any single armor points, therefore it's set to fractional accounting precision.";

    public string MechLabMechInfoWidgetFormat = "0.###";
    public string MechLabMechInfoWidgetToolTipFormat = "0.######";
    public string MechLabComponentFormat = "0.###";

    public float UnderweightWarningThreshold = 1f;
    public string UnderweightWarningThresholdDescription = "How many tons a mech has to be underweight to show a warning";
}