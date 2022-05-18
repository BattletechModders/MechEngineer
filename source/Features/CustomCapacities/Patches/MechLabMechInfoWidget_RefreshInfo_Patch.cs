using System;
using System.Globalization;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using Harmony;
using MechEngineer.Features.MechLabSlots;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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
            var layoutHardpoints = objStatus.Find("layout_hardpoints");
            MechLabLayoutUtils.NormalizeRectTransform(layoutHardpoints.gameObject);
            var container = Object.Instantiate(layoutHardpoints.gameObject, null);
            container.name = "custom_capacities";
            customCapacities = container.transform;
            var horizontal = container.GetComponent<HorizontalLayoutGroup>();
            if (horizontal != null)
            {
                Object.DestroyImmediate(horizontal);
            }
            foreach (var child in customCapacities.GetChildren())
            {
                Object.Destroy(child.gameObject);
            }

            var vlg = MechLabLayoutUtils.NormalizeVerticalLayoutGroup(container);
            vlg.padding = new(10, 10, 5, 5);
            vlg.spacing = 8;
            MechLabLayoutUtils.NormalizeContentSizeFitter(container);
            MechLabLayoutUtils.NormalizeRectTransform(container);

            customCapacities.SetParent(objStatus, false);
            customCapacities.SetSiblingIndex(1);
        }

        void SetCapacity(CustomCapacitiesSettings.CustomCapacity customCapacity, ref int shownCounter)
        {
            var customCapacityTransform = customCapacities.Find(customCapacity.Collection);
            if (customCapacityTransform == null)
            {
                var go = Object.Instantiate(remainingTonnage.gameObject, null);
                go.name = customCapacity.Collection;

                MechLabLayoutUtils.NormalizeLayoutElement(go, 83 + MechLabLayoutUtils.Shifted, 30);
                MechLabLayoutUtils.NormalizeRectTransform(go);
                MechLabLayoutUtils.NormalizeContentSizeFitter(go);
                MechLabLayoutUtils.NormalizeLocalizableText(go, 12);

                if (customCapacity.ToolTipHeader != null && customCapacity.ToolTipBody != null)
                {
                    var tooltip = go.AddComponent<HBSTooltip>();
                    tooltip.defaultStateData.SetObject(new BaseDescriptionDef
                    {
                        Name = customCapacity.ToolTipHeader,
                        Details = customCapacity.ToolTipBody
                    });
                }

                customCapacityTransform = go.transform;
                customCapacityTransform.SetParent(customCapacities, false);
            }

            {
                CustomCapacitiesFeature.CalculateCustomCapacityResults(mechDef, customCapacity.Collection, out var capacity, out var usage, out var hasError);

                var hideIfNotUsed = customCapacity.HideIfNoUsageAndCapacity && PrecisionUtils.Equals(capacity, 0) && PrecisionUtils.Equals(usage, 0);
                var go = customCapacityTransform.gameObject;
                if (!hideIfNotUsed)
                {
                    shownCounter++;
                }
                go.SetActive(!hideIfNotUsed);

                var text = customCapacity.Label
                    + "\n"
                    + usage.ToString(customCapacity.Format, CultureInfo.InvariantCulture)
                    + " / "
                    + capacity.ToString(customCapacity.Format, CultureInfo.InvariantCulture)
                    ;
                var color = hasError ? UIColor.Red : UIColor.White;
                SetText(
                    go,
                    text,
                    color
                );
            }
        }

        var shownCounter = 0;
        SetCapacity(CustomCapacitiesFeature.Shared.Settings.CarryWeight, ref shownCounter);
        foreach (var customCapacity in CustomCapacitiesFeature.Shared.Settings.CustomCapacities)
        {
            SetCapacity(customCapacity, ref shownCounter);
        }
        customCapacities.gameObject.SetActive(shownCounter > 0);
    }

    private static void SetText(GameObject go, string text, UIColor color)
    {
        var textComponent = go.GetComponent<LocalizableText>();
        textComponent.SetText(text);
        var colorTracker = go.GetComponent<UIColorRefTracker>();
        colorTracker.SetUIColor(color);
    }
}
