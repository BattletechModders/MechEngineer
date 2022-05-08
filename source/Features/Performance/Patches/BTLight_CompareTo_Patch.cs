using System;
using BattleTech.Rendering;
using Harmony;

namespace MechEngineer.Features.Performance.Patches;

// only sort by type and not also by InstanceID
[HarmonyPatch(typeof(BTLight), nameof(BTLight.CompareTo))]
public static class BTLight_CompareTo_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(BTLight __instance, object obj, ref int __result)
    {
        try
        {
            var one = __instance;
            var other = obj as BTLight;
            __result = (int)one.lightType - (int)other!.lightType;
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}