using System.Linq;
using BattleTech.UI;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.OnMaxArmor))]
public static class MechLabPanel_OnMaxArmor_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabPanel __instance, MechLabMechInfoWidget ___mechInfoWidget, MechLabItemSlotElement ___dragItem)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (__instance.Initialized && ___dragItem == null && !LocationExtensions.ChassisLocationList.Any(location => __instance.GetLocationWidget(location).IsDestroyed))
        {
            ArmorMaximizerHandler.OnMaxArmor(__instance, ___mechInfoWidget);
            __runOriginal = false;
        }
    }
}
