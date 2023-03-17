using BattleTech.UI;
using TMPro;

namespace MechEngineer.Features.OverrideTonnage.Patches;

[HarmonyPatch(typeof(MechLabMechInfoWidget), nameof(MechLabMechInfoWidget.CalculateTonnage))]
public static class MechLabMechInfoWidget_CalculateTonnage_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, 
        MechLabPanel ___mechLab,
        ref float ___currentTonnage,
        TextMeshProUGUI ___totalTonnage,
        UIColorRefTracker ___totalTonnageColor,
        TextMeshProUGUI ___remainingTonnage,
        UIColorRefTracker ___remainingTonnageColor)
    {
        if (!__runOriginal)
        {
            return;
        }

        var mechDef = ___mechLab.CreateMechDef();
        if (mechDef == null)
        {
            __runOriginal = false;
            return;
        }

        WeightsHandler.AdjustInfoWidget(
            mechDef,
            ___remainingTonnageColor,
            ___totalTonnageColor,
            ___totalTonnage,
            ___remainingTonnage,
            out ___currentTonnage
        );
        __runOriginal = false;
    }
}
