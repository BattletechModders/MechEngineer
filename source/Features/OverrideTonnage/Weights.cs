using System.Linq;
using BattleTech;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Engines.Helper;

namespace MechEngineer.Features.OverrideTonnage;

internal class Weights
{
    internal static float CalculateFreeTonnage(MechDef mechDef)
    {
        var weights = new Weights(mechDef);
        Control.Logger.Debug?.Log($"Chassis={mechDef.Chassis.Description.Id} weights={weights}");
        return weights.FreeWeight;
    }

    internal static float CalculateTotalTonnage(MechDef mechDef)
    {
        var weights = new Weights(mechDef);
        return weights.TotalWeight;
    }

    internal static float CalculateWeightFactorsChange(MechDef mechDef, WeightFactors componentFactors)
    {
        var weights = new Weights(mechDef);
        var before = weights.TotalWeight;
        weights.Factors.Combine(componentFactors);
        var after = weights.TotalWeight;
        return after - before;
    }

    internal float StandardArmorWeight { private get; set; }
    private readonly float StandardStructureWeight;
    private readonly float StandardChassisWeightCapacity;
    private readonly float ComponentSumWeight;
    private readonly Engine? Engine;

    // must reference Engine factors if they exist
    private readonly WeightFactors Factors;

    internal Weights(MechDef mechDef)
    {
        StandardArmorWeight = mechDef.StandardArmorTonnage();
        StandardStructureWeight = mechDef.Chassis.InitialTonnage;
        StandardChassisWeightCapacity = mechDef.Chassis.Tonnage;
        Engine = mechDef.GetEngine();
        Factors = Engine?.WeightFactors ?? WeightsUtils.GetWeightFactorsFromInventory(mechDef.Inventory);
        ComponentSumWeight = mechDef.Inventory.Sum(mechComponentRef => mechComponentRef.Def.Tonnage) - (Engine?.CoreDef.Def.Tonnage ?? 0);
    }

    private float TotalWeight => ArmorWeight + StructureWeight + EngineWeight + ComponentSumWeight;
    internal float FreeWeight => ChassisWeightCapacity - TotalWeight;

    private float ArmorWeight => CalculateWeight(StandardArmorWeight, Factors.ArmorFactor, ArmorRoundingPrecision);
    private float StructureWeight => CalculateWeight(StandardStructureWeight, Factors.StructureFactor);
    private float EngineWeight => Engine?.TotalTonnage ?? 0;
    private float ChassisWeightCapacity => CalculateWeight(StandardChassisWeightCapacity, Factors.ChassisFactor);

    private float ArmorRoundingPrecision => PrecisionUtils.RoundUp(StandardArmorRoundingPrecision * Factors.ArmorFactor, 0.0001f);

    private readonly float StandardArmorRoundingPrecision =
        OverrideTonnageFeature.settings.ArmorRoundingPrecision
        ?? UnityGameInstance.BattleTechGame.MechStatisticsConstants.TONNAGE_PER_ARMOR_POINT;

    private static float CalculateWeight(float unmodified, float factor, float? precision = null)
    {
        var modified = unmodified * factor;
        var modifiedRounded = PrecisionUtils.RoundUp(modified, precision ?? OverrideTonnageFeature.settings.TonnageStandardPrecision);
        return modifiedRounded;
    }

    public override string ToString()
    {
        return $"[capacity={ChassisWeightCapacity} total={TotalWeight} armor={ArmorWeight} structure={StructureWeight} components={ComponentSumWeight} engine={Engine?.EngineTonnage}]";
    }
}
