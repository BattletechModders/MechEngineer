using BattleTech.Rendering;

namespace MechEngineer.Features.Performance.Patches;

// only sort by type and not also by InstanceID
[HarmonyPatch(typeof(BTLight), nameof(BTLight.CompareTo))]
public static class BTLight_CompareTo_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, BTLight __instance, object obj, ref int __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        var one = __instance;
        var other = obj as BTLight;
        __result = (int)one.lightType - (int)other!.lightType;
        __runOriginal = false;
    }
}
