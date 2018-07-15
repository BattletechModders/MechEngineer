using System;
using BattleTech;
using BattleTech.UI;
using Harmony;
using TMPro;

namespace MechEngineer
{
    [HarmonyPatch(typeof(Mech), "GetComponentInSlot")]
    internal static class Mech_GetComponentInSlot_Patch
    {
        // TODO write transpiler
        // list.Add(mechComponent);
        // List<MechComponent> list = new List<MechComponent>();
        // ignore Add

        /*
                if (__instance.mechComponentRef.Is<Flags>(out var f) && f.IsSet("reroll_weapons_hit"))
                {
                    // slots can't be damaged
                    MechCheckForCritPatch.Message = null;
                    return false;
                }
         */
    }
}