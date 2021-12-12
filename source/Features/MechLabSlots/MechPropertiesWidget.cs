using System;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using Harmony;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MechEngineer.Features.MechLabSlots
{
    public class MechPropertiesWidget
    {
        private static MechLabLocationWidget PropertiesWidget;

        internal static bool IsCustomWidget(MechLabLocationWidget widget)
        {
            return PropertiesWidget == widget;
        }

        internal static void Setup(MechLabPanel mechLabPanel)
        {
            var armWidget = mechLabPanel.rightArmWidget;
            GameObject go;
            if (PropertiesWidget == null)
            {
                var template = mechLabPanel.centerTorsoWidget;

                go = Object.Instantiate(template.gameObject, null);
                go.name = "MechPropertiesWidget";
                go.SetActive(MechLabSlotsFeature.settings.MechLabGeneralWidgetEnabled);
                {
                    var vlg = go.GetComponent<VerticalLayoutGroup>();
                    vlg.padding = new RectOffset(0, 0, 0, 3);
                    vlg.spacing = 4;
                }

                go.transform.GetChild("layout_armor").gameObject.SetActive(false);
                go.transform.GetChild("layout_hardpoints").gameObject.SetActive(false);
                go.transform.GetChild("layout_locationText").GetChild("txt_structure").gameObject.SetActive(false);
                go.transform.GetChild("layout_locationText").GetChild("txt_location").GetComponent<TextMeshProUGUI>().text = MechLabSlotsFeature.settings.MechLabGeneralWidgetLabel;

                PropertiesWidget = go.GetComponent<MechLabLocationWidget>();
            }
            else
            {
                go = PropertiesWidget.gameObject;
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

            PropertiesWidget.Init(mechLabPanel);

            var layout = new WidgetLayout(PropertiesWidget);
            MechLabSlotsFixer.ModifyLayoutSlotCount(layout, MechLabSlotsFeature.settings.MechLabGeneralSlots);
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
                if (IsMechConfiguration(element.ComponentRef.Def))
                {
                    var inventoryParent = Traverse
                        .Create(PropertiesWidget)
                        .Field<Transform>("inventoryParent")
                        .Value;
                    @this.SetParent(inventoryParent, worldPositionStays);
                    return;
                }
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }

            @this.SetParent(parent, worldPositionStays);
        }

        internal static bool IsMechConfiguration(MechComponentDef def)
        {
            return MechLabSlotsFeature.settings.MechLabGeneralWidgetEnabled && def.Is<MechConfiguration>();
        }

        internal static bool OnDrop(MechLabLocationWidget widget, PointerEventData eventData)
        {
            if (widget == PropertiesWidget)
            {
                var mechLab = (MechLabPanel) widget.parentDropTarget;
                mechLab.centerTorsoWidget.OnDrop(eventData);
                return true;
            }
            return false;
        }

        internal static void RefreshDropHighlights(MechLabLocationWidget widget, IMechLabDraggableItem item)
        {
            if (item == null)
            {
                PropertiesWidget.ShowHighlightFrame(false);
            }
        }

        internal static void ShowHighlightFrame(MechLabLocationWidget widget, ref MechComponentRef cRef)
        {
            if (cRef == null)
            {
                return;
            }

            if (widget == PropertiesWidget)
            {
                return;
            }

            if (!IsMechConfiguration(cRef?.Def))
            {
                return;
            }

            cRef = null;
            PropertiesWidget.ShowHighlightFrame(true);
        }
    }
}