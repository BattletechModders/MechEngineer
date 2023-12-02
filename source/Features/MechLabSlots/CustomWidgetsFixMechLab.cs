using System;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MechEngineer.Features.MechLabSlots;

public class CustomWidgetsFixMechLab
{
    private static MechLabLocationWidget? TopLeftWidget;
    private static MechLabLocationWidget? TopRightWidget;

    internal static bool IsCustomWidget(MechLabLocationWidget widget)
    {
        return widget == TopLeftWidget || widget == TopRightWidget;
    }

    internal static void Setup(MechLabPanel mechLabPanel)
    {
        SetupWidget(
            "TopLeftWidget",
            ref TopLeftWidget,
            mechLabPanel,
            mechLabPanel.rightArmWidget,
            MechLabSlotsFeature.settings.TopLeftWidget
        );

        SetupWidget(
            "TopRightWidget",
            ref TopRightWidget,
            mechLabPanel,
            mechLabPanel.leftArmWidget,
            MechLabSlotsFeature.settings.TopRightWidget
        );
    }

    internal static void SetupWidget(string id,
        ref MechLabLocationWidget? topWidget,
        MechLabPanel mechLabPanel,
        MechLabLocationWidget armWidget,
        MechLabSlotsSettings.WidgetSettings settings)
    {
        GameObject go;
        if (topWidget == null)
        {
            var template = mechLabPanel.centerTorsoWidget;

            go = Object.Instantiate(template.gameObject, null);
            go.name = id;
            go.SetActive(settings.Enabled);
            {
                var vlg = go.GetComponent<VerticalLayoutGroup>();
                vlg.padding = new RectOffset(0, 0, 0, 3);
                vlg.spacing = 4;
            }

            go.transform.Find("layout_armor").gameObject.SetActive(false);
            go.transform.Find("layout_hardpoints").gameObject.SetActive(false);
            go.transform.Find("layout_locationText/txt_structure").gameObject.SetActive(false);
            go.transform.Find("layout_locationText/txt_location").GetComponent<TextMeshProUGUI>().text = settings.Label;

            topWidget = go.GetComponent<MechLabLocationWidget>();
        }
        else
        {
            go = topWidget.gameObject;
            go.SetActive(settings.Enabled);
        }

        var parent = armWidget.transform.parent;
        go.transform.SetParent(parent, false);
        go.transform.SetAsFirstSibling();
        go.GetComponent<LayoutElement>().ignoreLayout = true;
        {
            var rect = go.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(0, 0);
            rect.pivot = new Vector2(0, 0);
            rect.anchoredPosition = new Vector2(0, -MechLabSlotsFeature.settings.MechLabArmTopPadding + 20);
        }
        {
            var clg = parent.GetComponent<VerticalLayoutGroup>();
            clg.padding = new RectOffset(0, 0, MechLabSlotsFeature.settings.MechLabArmTopPadding, 0);
        }

        topWidget.Init(mechLabPanel);

        // allow modify layout to go to 0 and
        var layout = new WidgetLayout(topWidget);
        MechLabSlotsFixer.ModifyLayoutSlotCount(layout, settings.Slots);

        {
            var mechRectTransform = parent.parent.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(mechRectTransform);
        }
    }

    internal static void OnAdditem_SetParent(Transform @this, Transform parent, bool worldPositionStays)
    {
        try
        {
            var element = @this.GetComponent<MechLabItemSlotElement>();
            var widget = MechWidgetLocation(element.ComponentRef.Def);
            if (widget != null)
            {
                var inventoryParent = widget.inventoryParent;
                @this.SetParent(inventoryParent, worldPositionStays);
                return;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }

        @this.SetParent(parent, worldPositionStays);
    }

    private static MechLabLocationWidget? MechWidgetLocation(MechComponentDef? def)
    {
        if (def != null && def.Is<CustomWidget>(out var config))
        {
            if (config.Location == CustomWidget.MechLabWidgetLocation.TopLeft
                && MechLabSlotsFeature.settings.TopLeftWidget.Enabled)
            {
                return TopLeftWidget;
            }

            if (config.Location == CustomWidget.MechLabWidgetLocation.TopRight
                && MechLabSlotsFeature.settings.TopRightWidget.Enabled)
            {
                return TopRightWidget;
            }
        }

        return null;
    }

    internal static bool OnDrop(MechLabLocationWidget widget, PointerEventData eventData)
    {
        if (IsCustomWidget(widget))
        {
            var mechLab = (MechLabPanel)widget.parentDropTarget;
            mechLab.centerTorsoWidget.OnDrop(eventData);
            return true;
        }

        return false;
    }

    internal static void RefreshDropHighlights(MechLabLocationWidget widget, IMechLabDraggableItem? item)
    {
        if (item == null)
        {
            TopLeftWidget!.ShowHighlightFrame(false);
            TopRightWidget!.ShowHighlightFrame(false);
        }
    }

    internal static bool ShowHighlightFrame(
        MechLabLocationWidget widget,
        bool isOriginalLocation,
        ref MechComponentRef? cRef
        )
    {
        if (cRef == null)
        {
            return true;
        }

        // we only want to highlight once, CT is only called once
        if (widget.loadout.Location != ChassisLocations.CenterTorso)
        {
            return true;
        }

        if (cRef.Def.CCFlags().NoRemove || cRef.IsFixed)
        {
            return true;
        }

        // get the correct widget to highlight
        var nwidget = MechWidgetLocation(cRef.Def);
        if (nwidget == null)
        {
            return true;
        }

        nwidget.ShowHighlightFrame(true, isOriginalLocation ? UIColor.Blue : UIColor.Gold);
        cRef = null;
        return false;
    }

    internal static void ShowOrHideCustomWidgets(MechLabLocationWidget widget)
    {
        if (TopLeftWidget == null || TopRightWidget == null)
        {
            Log.Main.Warning?.Log("Top widgets not initialized even though they should be");
            return;
        }

        if (widget.loadout.Location != ChassisLocations.CenterTorso)
        {
            return;
        }

        var custom = widget.mechLab.activeMechDef.Chassis.GetComponent<CustomWidgetChassisCustom>();
        static void ShowOrHideCustomWidget(
            MechLabLocationWidget customWidget,
            bool? enabled,
            MechLabSlotsSettings.WidgetSettings settings
        )
        {
            customWidget.gameObject.SetActive(enabled ?? settings.Enabled);
        }
        ShowOrHideCustomWidget(TopLeftWidget, custom?.TopLeftWidgetEnabled, MechLabSlotsFeature.settings.TopLeftWidget);
        ShowOrHideCustomWidget(TopRightWidget, custom?.TopRightWidgetEnabled, MechLabSlotsFeature.settings.TopRightWidget);
    }
}
