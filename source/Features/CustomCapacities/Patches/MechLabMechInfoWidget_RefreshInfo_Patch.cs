using System;
using System.Globalization;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using Harmony;
using MechEngineer.Features.MechLabSlots;
using MechEngineer.Features.OverrideTonnage;
using TMPro;
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

    // to allow more space for capacities, we could move elements around
    private const float LeftShift = 0; // 10;
    private const float RightShift = 0; // 30;

    private static void SetupCapacitiesLayout(MechDef mechDef, LocalizableText remainingTonnage)
    {

        var layoutTonnage = remainingTonnage.transform.parent;
        var objStatus = layoutTonnage.parent;
        var customCapacities = objStatus.Find("custom_capacities");

        if (customCapacities == null)
        {
            { // TODO move to MechLabSlots
                var go = layoutTonnage.gameObject;
                FixLayoutElement(go, 220, 69);
                FixRectTransform(go);
            }

            { // TODO move to MechLabSlots
                var go = objStatus.gameObject;
                FixVerticalLayoutGroup(go).spacing = 5;

                go.GetComponent<RectTransform>().anchoredPosition = new(515 - LeftShift, 0);

                {
                    var objMeta = objStatus.parent;
                    var objGroupLeft = objMeta.parent;
                    var representation = objGroupLeft.parent;
                    {
                        var objWarnings = representation.Find("OBJ_warnings");
                        objWarnings.GetComponent<RectTransform>().anchoredPosition = new(300 + RightShift, -32);
                    }
                    {
                        var objMech = representation.Find("OBJ_mech");
                        objMech.GetComponent<RectTransform>().anchoredPosition = new(300 + RightShift, -185);
                    }
                    {
                        var objActions = representation.Find("OBJ_actions");
                        objActions.GetComponent<RectTransform>().anchoredPosition = new(588 + RightShift, -185);
                    }
                }
            }

            {
                var layoutHardpoints = objStatus.Find("layout_hardpoints");
                FixRectTransform(layoutHardpoints.gameObject);
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

                var vlg = FixVerticalLayoutGroup(container);
                vlg.padding = new(10, 10, 5, 5);
                vlg.spacing = 8;
                FixContentSizeFitter(container);
                FixRectTransform(container);

                customCapacities.SetParent(objStatus, false);
                customCapacities.SetSiblingIndex(1);
            }
        }

        void SetCapacity(CustomCapacitiesSettings.CustomCapacity customCapacity)
        {
            var customCapacityTransform = customCapacities.Find(customCapacity.Collection);
            if (customCapacityTransform == null)
            {
                var go = Object.Instantiate(remainingTonnage.gameObject, null);
                go.name = customCapacity.Collection;

                FixLayoutElement(go, 85 + LeftShift + RightShift, 30);
                FixRectTransform(go);
                FixContentSizeFitter(go);
                FixLocalizableText(go);

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

        SetCapacity(CustomCapacitiesFeature.Shared.Settings.CarryWeight);
        foreach (var customCapacity in CustomCapacitiesFeature.Shared.Settings.CustomCapacities)
        {
            SetCapacity(customCapacity);
        }
    }

    private static void SetText(GameObject go, string text, UIColor color)
    {
        var textComponent = go.GetComponent<LocalizableText>();
        textComponent.SetText(text);
        var colorTracker = go.GetComponent<UIColorRefTracker>();
        colorTracker.SetUIColor(color);
    }

    private static void FixLocalizableText(GameObject go)
    {
        var lt = go.GetComponent<LocalizableText>();
        lt.fontSize = lt.fontSizeMax = lt.fontSizeMin = 12;
        lt.enableAutoSizing = false;
        lt.autoSizeTextContainer = false;
        lt.alignment = TextAlignmentOptions.Left;
        lt.lineSpacing = 3;
    }

    private static VerticalLayoutGroup FixVerticalLayoutGroup(GameObject go)
    {
        var component = go.GetComponent<VerticalLayoutGroup>() ?? go.AddComponent<VerticalLayoutGroup>();
        component.enabled = true;
        component.childControlHeight = true;
        component.childControlWidth = true;
        component.childForceExpandHeight = false;
        component.childForceExpandWidth = false;
        component.padding = new(0, 0, 0, 0);
        component.spacing = 0;
        component.childAlignment = TextAnchor.UpperLeft;
        return component;
    }

    private static void FixContentSizeFitter(GameObject go)
    {
        var component = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
        component.horizontalFit = ContentSizeFitter.FitMode.MinSize;
        component.verticalFit = ContentSizeFitter.FitMode.MinSize;
    }

    private static void FixLayoutElement(GameObject go, float width, float height)
    {
        var component = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
        component.ignoreLayout = false;
        component.enabled = true;
        component.preferredWidth = component.minWidth = width;
        component.preferredHeight = component.minHeight = height;
    }

    private static void FixRectTransform(GameObject go)
    {
        var component = go.GetComponent<RectTransform>();
        component.anchorMin = new(0, 1);
        component.anchorMax = new(0, 1);
        component.pivot = new(0, 1);
        component.anchoredPosition = new(0, 0);
    }
}
