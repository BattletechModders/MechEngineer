using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideDescriptions.Patches
{
    [HarmonyPatch(typeof(MechDef), "RefreshChassis")]
    public static class MechDef_RefreshChassis_Patch
    {
        public static void Postfix(MechDef __instance)
        {
            try
            {
                var mechDef = __instance;
                Traverse.Create(mechDef)
                    .Property<string>(nameof(DescriptionDef.Details))
                    .Value = mechDef.Chassis.Description.Details;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}