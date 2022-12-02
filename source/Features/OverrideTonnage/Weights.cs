using System.Linq;
using BattleTech;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Misc;

namespace MechEngineer.Features.OverrideTonnage;

internal class Weights
{
    internal readonly float ArmorAssignedPoints;
    internal readonly float ArmorPerTon;
    internal readonly float StandardArmorWeight;
    internal readonly float StandardStructureWeight;
    internal readonly float StandardChassisWeightCapacity;
    [ToString]
    internal readonly float ComponentSumWeight;
    internal readonly Engine? Engine;

    // must reference Engine factors if they exist
    internal readonly WeightFactors Factors;

    internal Weights(MechDef mechDef, bool processInventory = true, bool processArmor = true)
    {
        if (processArmor)
        {
            ArmorAssignedPoints = mechDef.MechDefAssignedArmor;
            ArmorPerTon = UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_TENTH_TON * 10f;
            StandardArmorWeight = ArmorAssignedPoints / ArmorPerTon;
        }
        StandardStructureWeight = mechDef.Chassis.InitialTonnage;
        StandardChassisWeightCapacity = mechDef.Chassis.Tonnage;
        Engine = mechDef.GetEngine();
        if (processInventory)
        {
            Factors = Engine?.WeightFactors ?? WeightsUtils.GetWeightFactorsFromInventory(mechDef.Inventory);
            ComponentSumWeight = mechDef.Inventory.Sum(mechComponentRef => mechComponentRef.Def.Tonnage) - (Engine?.CoreDef.Def.Tonnage ?? 0);
        }
        else
        {
            Factors = new();
        }
    }

    [ToString]
    internal float TotalWeight => ArmorWeight + StructureWeight + EngineWeight + ComponentWeightByChassisFactor + ComponentSumWeight;
    [ToString]
    internal float FreeWeight => ChassisWeightCapacity - TotalWeight;

    [ToString]
    internal float ArmorWeight => PrecisionUtils.RoundUp(StandardArmorWeight * Factors.ArmorFactor, OverrideTonnageFeature.settings.ArmorRoundingPrecision);
    [ToString]
    internal float StructureWeight => PrecisionUtils.RoundUp(StandardStructureWeight * Factors.StructureFactor, OverrideTonnageFeature.settings.TonnageStandardPrecision);
    [ToString]
    internal float EngineWeight => Engine?.TotalTonnage ?? 0;
    [ToString]
    internal float ChassisWeightCapacity => PrecisionUtils.RoundUp(StandardChassisWeightCapacity * Factors.ChassisCapacityFactor, OverrideTonnageFeature.settings.TonnageStandardPrecision);
    [ToString]
    internal float ComponentWeightByChassisFactor => PrecisionUtils.RoundUp(StandardChassisWeightCapacity * Factors.ComponentByChassisFactor, OverrideTonnageFeature.settings.TonnageStandardPrecision);

    public override string ToString()
    {
        return ToStringBuilder.ToString(this);
    }
}