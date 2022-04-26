using System.Linq;
using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.OverrideTonnage;

public static class WeightsUtils
{
    // TODO remove code duplication possible?
    // TODO not properly correct as not the same as other calculations
    internal static float CalculateArmorFactor(MechDef mechDef)
    {
        if (mechDef?.Inventory == null)
        {
            return 0;
        }

        var armorFactor = mechDef.Inventory
                              .Select(r => r.Def?.GetComponent<Weights>())
                              .Where(w => w != null)
                              .Sum(weights => weights.ArmorFactor - 1)
                          + 1;
        return armorFactor;
    }
    
    internal static float CalculateTonnage(MechDef mechDef)
    {
        var tonnage = mechDef.Chassis.InitialTonnage;
        tonnage += mechDef.MechDefAssignedArmor / (UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_TENTH_TON * 10f);
        tonnage += mechDef.Inventory.Sum(mechComponentRef => mechComponentRef.Def.Tonnage);
        tonnage += WeightsHandler.Shared.TonnageChanges(mechDef);
        return tonnage;
    }
    
    internal static float CalculateFreeTonnage(MechDef mechDef)
    {
        var freeTonnage = mechDef.Chassis.Tonnage - CalculateTonnage(mechDef);
        Control.Logger.Debug?.Log($" Chassis tonnage={mechDef.Chassis.Tonnage} initialTonnage={mechDef.Chassis.InitialTonnage} armorTonnage={mechDef.StandardArmorTonnage()} freeTonnage={freeTonnage}");
        return freeTonnage;
    }

    internal static float StandardArmorTonnage(this MechDef mechDef)
    {
        var armorPerTon = UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_TENTH_TON * 10f;
        return mechDef.MechDefAssignedArmor / armorPerTon;
    }
}