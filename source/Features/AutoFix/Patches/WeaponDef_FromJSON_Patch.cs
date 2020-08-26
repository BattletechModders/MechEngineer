using System;
using System.Linq;
using BattleTech;
using CustomComponents;
using ErosionBrushPlugin;
using Harmony;

namespace MechEngineer.Features.AutoFix.Patches
{
    [HarmonyPatch(typeof(WeaponDef), nameof(Weapon.FromJSON))]
    public static class WeaponDef_FromJSON_Patch
    {
        public static void Postfix(WeaponDef __instance)
        {
            try
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
                            Traverse.Create(def).Property("InventorySize").SetValue(newValue.Value);
                        }

                        if (change.TonnageChange != null)
                        {
                            var newValue = change.TonnageChange.Change(def.Tonnage);
                            if (!newValue.HasValue)
                            {
                                return;
                            }
                            Traverse.Create(def).Property("Tonnage").SetValue(newValue.Value);
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
                        Traverse.Create(def).Property("InventorySize").SetValue(fixedSize);

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
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}
