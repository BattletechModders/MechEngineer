using System;
using BattleTech.UI;
using Harmony;
using TMPro;

namespace MechEngineer.Features.OverrideTonnage.Patches;

[HarmonyPatch(typeof(MechLabMechInfoWidget), nameof(MechLabMechInfoWidget.CalculateTonnage))]
public static class MechLabMechInfoWidget_CalculateTonnage_Patch
{
    public static bool Prefix(
        MechLabPanel ___mechLab,
        ref float ___currentTonnage,
        TextMeshProUGUI ___totalTonnage,
        UIColorRefTracker ___totalTonnageColor,
        TextMeshProUGUI ___remainingTonnage,
        UIColorRefTracker ___remainingTonnageColor)
    {
        try
        {
            var mechDef = ___mechLab.CreateMechDef();
            if (mechDef == null)
            {
                return false;
            }

            WeightsHandler.AdjustInfoWidget(
                mechDef,
                ___remainingTonnageColor,
                ___totalTonnageColor,
                ___totalTonnage,
                ___remainingTonnage,
                out ___currentTonnage
            );
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}