using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(typeof(MechLabLocationWidget), nameof(MechLabLocationWidget.SetData))]
public static class MechLabLocationWidget_SetData_Patch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.MethodReplacer(
            AccessTools.Method(typeof(Transform), nameof(Transform.SetParent),
                new[] {typeof(Transform), typeof(bool)}),
            AccessTools.Method(typeof(CustomWidgetsFixMechLab),
                nameof(CustomWidgetsFixMechLab.OnAdditem_SetParent))
        );
    }

    [HarmonyPostfix]
    public static void Postfix(MechLabLocationWidget __instance, int ___maxSlots, ref LocationLoadoutDef loadout)
    {
        try
        {
            var widget = __instance;

            var widgetLayout = new WidgetLayout(widget);
            MechLabSlotsFixer.FixSlots(widgetLayout, ___maxSlots);
            DynamicSlotsFeature.PrepareWidget(widgetLayout);
            AdjustMechLabLocationNaming(widget, loadout.Location);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }

    private static void AdjustMechLabLocationNaming(MechLabLocationWidget widget, ChassisLocations location)
    {
        // just hide armor = 0 stuff
        widget.gameObject.SetActive(!ShouldHide(widget));

        var mechLab = (MechLabPanel)widget.parentDropTarget;
        var text = ChassisLocationNamingUtils.GetLocationLabel(mechLab.activeMechDef.Chassis, location);

        widget.locationName.SetText(text);
    }

    // hide any location with maxArmor <= 0 && structure <= 1
    // for vehicles and troopers
    private static bool ShouldHide(MechLabLocationWidget widget)
    {
        var def = widget.chassisLocationDef;
        return PrecisionUtils.SmallerOrEqualsTo(def.MaxArmor, 0)
               && PrecisionUtils.SmallerOrEqualsTo(def.InternalStructure, 1);
    }
}