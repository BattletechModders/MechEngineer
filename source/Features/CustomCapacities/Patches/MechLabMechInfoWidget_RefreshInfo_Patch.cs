using System;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;
using MechEngineer.Features.MechLabSlots;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.CustomCapacities.Patches;

[HarmonyPatch(typeof(MechLabMechInfoWidget), nameof(MechLabMechInfoWidget.RefreshInfo))]
public static class MechLabMechInfoWidget_RefreshInfo_Patch
{
    [HarmonyPostfix]
    public static void Postfix(
        MechLabPanel ___mechLab,
        LocalizableText ___remainingTonnage)
    {
        try
        {
            var mechDef = ___mechLab.CreateMechDef();
            if (mechDef == null)
            {
                return;
            }

            SetupCapacitiesLayout(mechDef, ___remainingTonnage);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }

    private static void SetupCapacitiesLayout(MechDef mechDef, LocalizableText remainingTonnage)
    {

        var layoutTonnage = remainingTonnage.transform.parent;
        var objStatus = layoutTonnage.parent;
        var customCapacities = objStatus.Find("custom_capacities");

        if (customCapacities == null)
        {
            {
                var layoutHardpoints = objStatus.Find("layout_hardpoints");
                MechLabLayoutUtils.NormalizeRectTransform(layoutHardpoints.gameObject);
            }

            var go = new GameObject("custom_capacities");

            go.AddComponent<Image>();
            var tracker = go.AddComponent<UIColorRefTracker>();
            tracker.SetUIColor(UIColor.DarkGray);

            var group = go.AddComponent<VerticalLayoutGroup>();
            group.childForceExpandHeight = group.childForceExpandWidth = false;
            group.childControlHeight = group.childControlWidth = true;
            group.childAlignment = TextAnchor.UpperRight;
            group.spacing = 5;
            group.padding = new(0, 0, 5, 5);

            var fitter = go.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            MechLabLayoutUtils.NormalizeRectTransform(go);

            customCapacities = go.transform;
            customCapacities.SetParent(objStatus, false);
            customCapacities.SetSiblingIndex(1);
        }

        void SetCapacity(CustomCapacitiesSettings.CustomCapacity customCapacity, ref int shownCounter)
        {
            var id = customCapacity.Description.Id;
            var element = customCapacities.Find(id)?.GetComponent<CustomCapacityUIElement>();

            if (element == null)
            {
                var go = new GameObject(id);
                element = go.AddComponent<CustomCapacityUIElement>();
                go.transform.SetParent(customCapacities, false);
            }

            {
                CustomCapacitiesFeature.Shared.CalculateCustomCapacityResults(
                    mechDef,
                    customCapacity,
                    out var description,
                    out var text,
                    out var color,
                    out var show
                );

                element.SetData(description, text, color);

                if (show)
                {
                    shownCounter++;
                }
                element.gameObject.SetActive(show);
            }
        }

        var shownCounter = 0;
        foreach (var customCapacity in CustomCapacitiesFeature.Shared.Settings.AllCapacities)
        {
            SetCapacity(customCapacity, ref shownCounter);
        }
        customCapacities.gameObject.SetActive(shownCounter > 0);
    }
}