using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.HardpointFix.sorting.Patches
{
    [HarmonyPatch(typeof(MechDef), "RefreshInventory")]
    public static class MechDef_RefreshInventory_Patch
    {
        public static void Prefix(MechDef __instance)
        {
            try
            {
                var adapter = new MechDefAdapter(__instance);
                if (adapter.Chassis?.HardpointDataDef == null)
                {
                    return;
                }

                var componentRefs = adapter.Inventory
                    .Where(c => c != null).Select(c =>
                    {
                        if (c.DataManager == null)
                        {
                            c.DataManager = adapter.DataManager;
                        }

                        c.RefreshComponentDef();

                        return c;
                    })
                    .Where(c => !c.hasPrefabName)
                    .ToList();
                MechHardpointRules_GetComponentPrefabName_Patch.SetupCalculator(adapter.Chassis, componentRefs);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        public static void Postfix(MechDef __instance)
        {
            MechHardpointRules_GetComponentPrefabName_Patch.ResetCalculator();
        }
    }
}