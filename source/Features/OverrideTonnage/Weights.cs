using System.Linq;
using BattleTech;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Engines.Helper;

namespace MechEngineer.Features.OverrideTonnage;

internal class Weights
{
    internal readonly float ArmorAssignedPoints;
    internal readonly float ArmorPerTon;
    internal readonly float StandardArmorWeight;
    internal readonly float StandardStructureWeight;
    internal readonly float StandardChassisWeightCapacity;
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

    internal float TotalWeight => ArmorWeight + StructureWeight + EngineWeight + ComponentSumWeight;
    internal float FreeWeight => ChassisWeightCapacity - TotalWeight;

    internal float ArmorWeight => PrecisionUtils.RoundUp(StandardArmorWeight *  Factors.ArmorFactor, OverrideTonnageFeature.settings.ArmorRoundingPrecision);
    internal float StructureWeight => PrecisionUtils.RoundUp(StandardStructureWeight *  Factors.StructureFactor, OverrideTonnageFeature.settings.TonnageStandardPrecision);
    internal float EngineWeight => Engine?.TotalTonnage ?? 0;
    internal float ChassisWeightCapacity => PrecisionUtils.RoundUp(StandardChassisWeightCapacity *  Factors.ChassisFactor, OverrideTonnageFeature.settings.TonnageStandardPrecision);

    public override string ToString()
    {
        return $"[capacity={ChassisWeightCapacity} total={TotalWeight} armor={ArmorWeight} structure={StructureWeight} components={ComponentSumWeight} engine={Engine?.EngineTonnage}]";
    }
}
