using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.MechLabSlots;

internal static class MechLabLayoutUtils
{
    internal static void FixMechLabLayouts(MechLabPanel panel)
    {
        var finder = new MechLabLayoutFinder(panel);
        FixCustomCapacitiesRelatedLayouts(finder);
        FixMechLabLocationWidgetLayouts(finder);
    }

    internal static float Shifted => LeftShift + RightShift;
    private static float LeftShift => MechLabSlotsFeature.settings.MechLabStatusLeftShift;
    private static float RightShift => MechLabSlotsFeature.settings.MechLabMechRightShift;

    private static void FixCustomCapacitiesRelatedLayouts(MechLabLayoutFinder finder)
    {
        var customCapacities = finder.ObjStatus.Find("custom_capacities");

        if (customCapacities != null)
        {
            return;
        }

        {
            var go = finder.LayoutTonnage.gameObject;
            NormalizeLayoutElement(go, 220, 69);
            NormalizeRectTransform(go);
        }

        NormalizeVerticalLayoutGroup(finder.ObjStatus.gameObject).spacing = 5;

        {
            var leftShift = LeftShift;
            finder.ObjStatus.GetComponent<RectTransform>().anchoredPosition = new(515 - leftShift, 0);
        }

        {
            var rightShift = RightShift;
            finder.ObjWarnings.GetComponent<RectTransform>().anchoredPosition = new(300 + rightShift, -32);
            finder.ObjMech.GetComponent<RectTransform>().anchoredPosition = new(300 + rightShift, -185);
            finder.ObjActions.GetComponent<RectTransform>().anchoredPosition = new(588 + rightShift, -185);
        }
    }

    private static void FixMechLabLocationWidgetLayouts(MechLabLayoutFinder finder)
    {
        foreach (Transform container in finder.ObjMech)
        {
            {
                var go = container.gameObject;
                EnableLayout(go);
                var component = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
                component.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                component.enabled = true;
                {
                    var clg = go.GetComponent<VerticalLayoutGroup>();
                    clg.padding = new RectOffset(0, 0, 0, 0);
                    clg.spacing = 56;
                }
            }

            foreach (Transform widget in container)
            {
                var widgetComponent = widget.GetComponent<MechLabLocationWidget>();
                if (widgetComponent == null || CustomWidgetsFixMechLab.IsCustomWidget(widgetComponent))
                {
                    continue;
                }

                EnableLayout(widget.gameObject);
                EnableLayout(widget.Find("layout_slots").gameObject);

                // fix different distances for lower bracket
                var rect = widget
                    .Find("layout_bg")
                    .Find("bracket_btm")
                    .GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0, 0);
                rect.offsetMin = new Vector2(0, -2);
                rect.offsetMax = new Vector2(0, 2);

                // put repair upper border ONTO widget lower border, since both color it fits and looks attached perfectly
                // saves pixels
                widget.Find("layout_repair")
                    .GetComponent<RectTransform>()
                    .anchoredPosition = new(0, -41);
            }
        }

        {
            var go = finder.ObjMech.gameObject;
            EnableLayout(go);
            var component = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
            component.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            component.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            component.enabled = true;
        }
    }

    internal static void EnableLayout(GameObject gameObject)
    {
        {
            var component = gameObject.GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
            component.ignoreLayout = false;
            component.enabled = true;
        }

        {
            var component = gameObject.GetComponent<HorizontalLayoutGroup>();
            if (component != null)
            {
                component.childForceExpandHeight = false;
                component.childAlignment = TextAnchor.UpperCenter;
                component.enabled = true;
            }
        }

        {
            var component = gameObject.GetComponent<VerticalLayoutGroup>();
            if (component != null)
            {
                component.enabled = true;
                component.childForceExpandHeight = false;
                component.childForceExpandWidth = false;
            }
        }

        {
            var component = gameObject.GetComponent<GridLayoutGroup>();
            if (component != null)
            {
                component.enabled = true;
            }
        }

        {
            var component = gameObject.GetComponent<ContentSizeFitter>();
            if (component != null)
            {
                component.enabled = true;
            }
        }
    }

    internal static LayoutElement NormalizeLayoutElement(GameObject go, float width, float height)
    {
        var component = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
        component.ignoreLayout = false;
        component.enabled = true;
        component.preferredWidth = component.minWidth = width;
        component.preferredHeight = component.minHeight = height;
        return component;
    }

    internal static RectTransform NormalizeRectTransform(GameObject go)
    {
        var component = go.GetComponent<RectTransform>();
        component.anchorMin = new(0, 1);
        component.anchorMax = new(0, 1);
        component.pivot = new(0, 1);
        component.anchoredPosition = new(0, 0);
        return component;
    }

    internal static VerticalLayoutGroup NormalizeVerticalLayoutGroup(GameObject go)
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

    internal static LocalizableText NormalizeLocalizableText(GameObject go, float fontSize)
    {
        var component = go.GetComponent<LocalizableText>();
        component.fontSize = component.fontSizeMax = component.fontSizeMin = fontSize;
        component.enableAutoSizing = false;
        component.autoSizeTextContainer = false;
        component.alignment = TextAlignmentOptions.Left;
        component.lineSpacing = 3;
        return component;
    }

    internal static ContentSizeFitter NormalizeContentSizeFitter(GameObject go)
    {
        var component = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
        component.horizontalFit = ContentSizeFitter.FitMode.MinSize;
        component.verticalFit = ContentSizeFitter.FitMode.MinSize;
        return component;
    }
}