using System.Collections.Generic;
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
        if (mechDef.Inventory == null)
        {
            return 0;
        }

        var armorFactor = mechDef.Inventory
                              .Select(r => r.Def?.GetComponent<WeightFactors>())
                              .Where(w => w != null)
                              .Select(w => w!)
                              .Sum(weights => weights.ArmorFactor - 1)
                          + 1;
        return armorFactor;
    }

    internal static float StandardArmorTonnage(this MechDef mechDef)
    {
        var armorPerTon = UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_TENTH_TON * 10f;
        return mechDef.MechDefAssignedArmor / armorPerTon;
    }

    internal static WeightFactors GetWeightFactorsFromInventory(IList<MechComponentRef> componentRefs)
    {
        var weightFactors = new WeightFactors();
        foreach (var componentRef in componentRefs)
        {
            var componentDef = componentRef.Def;
            if (componentDef.Is<WeightFactors>(out var weightSavings))
            {
                weightFactors.Combine(weightSavings);
            }
        }
        return weightFactors;
    }
}