using BattleTech.UI;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

[HarmonyPatch(typeof (MechLabLocationWidget), nameof(MechLabLocationWidget.OnRearArmorAdd))]
public static class MechLabLocationWidget_OnRearArmorAdd_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabLocationWidget __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        ArmorMaximizerHandler.OnArmorAddOrSubtract(__instance, true, +1f);
        __runOriginal = false;
    }
}
