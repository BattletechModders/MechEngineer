using System;
using BattleTech.UI;
using Harmony;
using MechEngineer.Features.MechLabSlots;
using MechEngineer.Helper;
using UnityEngine.UI;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

[HarmonyPatch(typeof(MechLabLocationWidget), nameof(MechLabLocationWidget.SetData))]
public static class MechLabLocationWidget_SetData_Patch
{
    public static void Postfix(MechLabLocationWidget __instance)
    {
        try
        {
            var widget = __instance;
            void Setup(LanceStat lanceStat, bool isRearArmor)
            {
                var child = lanceStat.transform.GetChild("hit_tooltip");
                var button = child.gameObject.GetComponent<Button>() ?? child.gameObject.AddComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => ArmorMaximizerHandler.OnBarClick(widget, isRearArmor));
                Control.Logger.Trace?.Log($"Added onClick Location={widget.chassisLocationDef.Location.GetShortString()} isRearArmor={isRearArmor}");
            }
            Setup(widget.armorBar, false);
            if (widget.useRearArmor)
            {
                Setup(widget.rearArmorBar, true);
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}
