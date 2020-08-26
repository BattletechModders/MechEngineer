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
                var details = mechDef.Chassis.Description.Details;

                Control.Logger.Debug?.Log($"id={mechDef.Description.Id} details={details}");

                var description = mechDef.Description;
                Traverse.Create(description)
                    .Property<string>(nameof(description.Details))
                    .Value = details;
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}