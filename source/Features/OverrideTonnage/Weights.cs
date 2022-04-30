#nullable enable
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

    internal static float CalculateWeightFactorsChange(MechDef mechDef, WeightFactors changedFactors)
    {
        var weights = new Weights(mechDef);
        var before = weights.TotalWeight;
        weights.SetFactors(changedFactors);
        var after = weights.TotalWeight;
        return after - before;
    }

    internal float StandardArmorWeight;
    internal float StandardStructureWeight;
    internal float StandardChassisWeightCapacity;
    internal float ComponentSumWeight;
    internal Engine? Engine;

    internal WeightFactors Factors;
    internal void SetFactors(WeightFactors value)
    {
        Factors = value;
        if (Engine != null)
        {
            Engine.WeightFactors = value;
        }
    }

    internal Weights(MechDef mechDef)
    {
        StandardArmorWeight = mechDef.StandardArmorTonnage();
        StandardStructureWeight = mechDef.Chassis.Tonnage / 10f;
        StandardChassisWeightCapacity = mechDef.Chassis.Tonnage;
        Engine = mechDef.GetEngine();
        Factors = Engine?.WeightFactors ?? WeightsUtils.GetWeightFactorsFromInventory(mechDef.Inventory);
        ComponentSumWeight = mechDef.Inventory.Sum(mechComponentRef => mechComponentRef.Def.Tonnage) - (Engine?.CoreDef.Def.Tonnage ?? 0);
    }

    internal float TotalWeight => ArmorWeight + StructureWeight + EngineWeight + ComponentSumWeight;
    internal float FreeWeight => ChassisWeightCapacity - TotalWeight;

    internal float ArmorWeight => CalculateWeight(StandardArmorWeight, Factors.ArmorFactor, ArmorRoundingPrecision);
    internal float StructureWeight => CalculateWeight(StandardStructureWeight, Factors.StructureFactor);
    internal float EngineWeight => Engine?.TotalTonnage ?? 0;
    internal float ChassisWeightCapacity => CalculateWeight(StandardChassisWeightCapacity, Factors.ChassisFactor);

    internal float ArmorRoundingPrecision => PrecisionUtils.RoundUp(StandardArmorRoundingPrecision * Factors.ArmorFactor, 0.0001f);

    internal float StandardArmorRoundingPrecision =
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
