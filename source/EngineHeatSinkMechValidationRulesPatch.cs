using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechValidationRules), "ValidateMechPosessesWeapons")]
    public static class EngineHeatSinkMechValidationRulesPatch
    {
        // invalidate mech loadouts that mix double and single heatsinks
        public static void Postfix(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            try
            {
                bool hasSingle = false, hasDouble = false;
                var mixed = mechDef.Inventory
                    .Where(c => c.ComponentDefType == ComponentType.HeatSink)
                    .Select(c => c.Def as HeatSinkDef)
                    .Where(c => c != null)
                    .Select(cd =>
                    {
                        if (cd.IsSingle())
                        {
                            hasSingle = true;
                        }
                        else if (cd.IsDouble())
                        {
                            hasDouble = true;
                        }
                        return cd;
                    })
                    .Any(c => hasSingle && hasDouble);

                if (mixed)
                {
                    errorMessages[MechValidationType.InvalidInventorySlots].Add("MIXED HEATSINKS: Standard and Double Heat Sinks cannot be mixed");
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}