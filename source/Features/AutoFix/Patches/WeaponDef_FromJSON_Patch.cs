using System.Linq;
using BattleTech;
using CustomComponents;
using ErosionBrushPlugin;
using MechEngineer.Helper;

namespace MechEngineer.Features.AutoFix.Patches;

[HarmonyPatch(typeof(WeaponDef), nameof(Weapon.FromJSON))]
public static class WeaponDef_FromJSON_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(WeaponDef __instance)
    {
        var def = __instance;

        if (def.ComponentTags.IgnoreAutofix())
        {
            return;
        }

        var changes = AutoFixerFeature.settings.AutoFixWeaponDefSlotsChanges;
        if (changes != null)
        {
            foreach (var change in changes.Where(x => x.Type == def.WeaponSubType))
            {
                if (change.SlotChange != null)
                {
                    var newValue = change.SlotChange.Change(def.InventorySize);
                    if (!newValue.HasValue)
                    {
                        return;
                    }
                    def.InventorySize = newValue.Value;
                }

                if (change.TonnageChange != null)
                {
                    var newValue = change.TonnageChange.Change(def.Tonnage);
                    if (!newValue.HasValue)
                    {
                        return;
                    }
                    def.Tonnage = newValue.Value;
                }
            }
        }

        if (AutoFixerFeature.settings.AutoFixWeaponDefSplitting)
        {
            var threshold = AutoFixerFeature.settings.AutoFixWeaponDefSplittingLargerThan;
            var fullSize = def.InventorySize;
            if (fullSize > threshold)
            {
                var fixedSize = AutoFixerFeature.settings.AutoFixWeaponDefSplittingFixedSize;
                def.InventorySize = fixedSize;

                if (!def.Is<DynamicSlots.DynamicSlots>(out var slots))
                {
                    var dynamicSlotTemplate = AutoFixerFeature.settings.AutoFixWeaponDefSplittingDynamicSlotTemplate;
                    slots = dynamicSlotTemplate.ReflectionCopy();
                    def.AddComponent(slots);
                }

                slots.ReservedSlots = fullSize - fixedSize;
            }
        }
    }
}
