using System;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
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

            CustomCapacitiesFeature.CalculateCarryWeight(mechDef, out var capacity, out var usage);

            var layoutTonnage = ___remainingTonnage.transform.parent;
            var objStatus = layoutTonnage.parent;
            var customCapacities = objStatus.Find("custom_capacities");

            if (customCapacities == null)
            {
                {
                    var go = layoutTonnage.gameObject;
                    FixLayoutElement(go, 220, 79);
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
                    FixVerticalLayoutGroup(container).padding = new(5, 5, 5, 5);;

                    FixContentSizeFitter(container);
                    FixRectTransform(container);

                    customCapacities.SetParent(objStatus, false);
                    customCapacities.SetSiblingIndex(1);
                }
            }

            {
                const string id = "carry_weight";
                var customCapacity = customCapacities.Find(id);
                if (customCapacity == null)
                {
                    var go = Object.Instantiate(___remainingTonnage.gameObject, null);
                    go.name = id;

                    FixLayoutElement(go, 90, 40);
                    FixRectTransform(go);
                    FixContentSizeFitter(go);
                    FixLocalizableText(go);

                    customCapacity = go.transform;
                    customCapacity.SetParent(customCapacities, false);

                    /*
                    LayoutRebuilder.ForceRebuildLayoutImmediate(objStatus.GetComponent<RectTransform>());
                    LayoutRebuilder.ForceRebuildLayoutImmediate(objStatus.GetComponent<RectTransform>());
                    LayoutRebuilder.ForceRebuildLayoutImmediate(objStatus.GetComponent<RectTransform>());
                    */
                }
                SetText(customCapacity.gameObject, CustomCapacitiesFeature.Shared.Settings.CarryWeightLabel, capacity, usage);
            }

            /*
            {
                const string id = "specialist";
                var customCapacity = customCapacities.Find(id);
                if (customCapacity == null)
                {
                    var go = Object.Instantiate(___remainingTonnage.gameObject, null);
                    go.name = id;

                    FixLayoutElement(go, 90, 40);
                    FixRectTransform(go);
                    FixContentSizeFitter(go);
                    FixLocalizableText(go);

                    customCapacity = go.transform;
                    customCapacity.SetParent(customCapacities, false);
                }
                SetText(customCapacity.gameObject, "Specialist", 13f, 0f);
            }
            */
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }

    private static void SetText(GameObject go, string label, float capacity, float usage)
    {
        var text = $"{label}\n{usage:0} / {capacity:0}";
        Control.Logger.Info?.Log(text);
        var textComponent = go.GetComponent<LocalizableText>();
        textComponent.SetText(text);
        var colorTracker = go.GetComponent<UIColorRefTracker>();
        colorTracker.SetUIColor(PrecisionUtils.SmallerThan(capacity, usage) ? UIColor.Red : UIColor.White);
    }

    private static void FixLocalizableText(GameObject go)
    {
        var lt = go.GetComponent<LocalizableText>();
        lt.fontSizeMin = 10;
        lt.fontSizeMax = 14;
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
