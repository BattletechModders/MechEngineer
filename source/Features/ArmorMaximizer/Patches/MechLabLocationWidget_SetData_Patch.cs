using BattleTech.UI;
using MechEngineer.Helper;
using UnityEngine.UI;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

[HarmonyPatch(typeof(MechLabLocationWidget), nameof(MechLabLocationWidget.SetData))]
public static class MechLabLocationWidget_SetData_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechLabLocationWidget __instance)
    {
        var widget = __instance;
        void Setup(LanceStat lanceStat, bool isRearArmor)
        {
            var child = lanceStat.transform.Find("hit_tooltip");
            var button = child.gameObject.GetComponent<Button>() ?? child.gameObject.AddComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ArmorMaximizerHandler.OnBarClick(widget, isRearArmor));
            Log.Main.Trace?.Log($"Added onClick Location={widget.chassisLocationDef.Location.GetShortString()} isRearArmor={isRearArmor}");
        }
        Setup(widget.armorBar, false);
        if (widget.useRearArmor)
        {
            Setup(widget.rearArmorBar, true);
        }
    }
}
