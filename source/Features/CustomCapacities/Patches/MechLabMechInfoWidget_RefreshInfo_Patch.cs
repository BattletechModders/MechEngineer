using System;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
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

    private static void SetupCapacitiesLayout(MechDef mechDef, LocalizableText remainingTonnage)
    {

        var layoutTonnage = remainingTonnage.transform.parent;
        var objStatus = layoutTonnage.parent;
        var customCapacities = objStatus.Find("custom_capacities");

        if (customCapacities == null)
        {
            {
                var go = layoutTonnage.gameObject;
                FixLayoutElement(go, 220, 69);
                FixRectTransform(go);
            }

            {
                var go = objStatus.gameObject;
                FixVerticalLayoutGroup(go).spacing = 5;
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

        void SetCapacity(string id, string label, string format, float capacity, float usage, bool hideIfNotUsed = false)
        {
            var customCapacityTransform = customCapacities.Find(id);
            if (customCapacityTransform == null)
            {
                var go = Object.Instantiate(remainingTonnage.gameObject, null);
                go.name = id;

                FixLayoutElement(go, 90, 30);
                FixRectTransform(go);
                FixContentSizeFitter(go);
                FixLocalizableText(go);

                customCapacityTransform = go.transform;
                customCapacityTransform.SetParent(customCapacities, false);
            }

            {
                var go = customCapacityTransform.gameObject;
                go.SetActive(!hideIfNotUsed);

                SetText(
                    go,
                    label,
                    format,
                    capacity,
                    usage
                );
            }
        }

        {
            CustomCapacitiesFeature.CalculateCarryWeightResults(mechDef, out var capacity, out var usage);
            SetCapacity(
                "carry_weight",
                CustomCapacitiesFeature.Shared.Settings.CarryTotalLabel,
                CustomCapacitiesFeature.Shared.Settings.CarryTotalFormat,
                capacity,
                usage
            );
        }

        foreach (var customCapacity in CustomCapacitiesFeature.Shared.Settings.CustomCapacities)
        {
            CustomCapacitiesFeature.CalculateCustomCapacityResults(mechDef, customCapacity.Collection, out var capacity, out var usage);
            var hideIfNotUsed = customCapacity.HideIfNoUsageAndCapacity && PrecisionUtils.Equals(capacity, 0) && PrecisionUtils.Equals(usage, 0);
            SetCapacity(
                customCapacity.Collection,
                customCapacity.Label,
                customCapacity.Format,
                capacity,
                usage,
                hideIfNotUsed
            );
        }
    }

    private static void SetText(GameObject go, string label, string format, float capacity, float usage)
    {
        var text = label + "\n" + string.Format(format, usage, capacity);
        var textComponent = go.GetComponent<LocalizableText>();
        textComponent.SetText(text);
        var colorTracker = go.GetComponent<UIColorRefTracker>();
        colorTracker.SetUIColor(PrecisionUtils.SmallerThan(capacity, usage) ? UIColor.Red : UIColor.White);
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
