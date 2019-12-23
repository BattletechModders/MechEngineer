using System;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using Harmony;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MechEngineer.Features.MechLabSlots
{
    public class MechPropertiesWidget
    {
        private static MechLabLocationWidget PropertiesWidget;

        internal static void Setup(MechLabPanel mechLabPanel)
        {
            if (PropertiesWidget != null)
            {
                PropertiesWidget.gameObject.transform.SetParent(mechLabPanel.rightArmWidget.transform, false);
                PropertiesWidget.Init(mechLabPanel);
                return;
            }

            {
                var template = mechLabPanel.centerTorsoWidget;
                var container = mechLabPanel.rightArmWidget.transform.parent.gameObject;
                var clg = container.GetComponent<VerticalLayoutGroup>();
                //clg.spacing = 20;
                clg.padding = new RectOffset(0, 0, MechLabSlotsFeature.settings.MechLabArmTopPadding, 0);
                var go = UnityEngine.Object.Instantiate(template.gameObject, null);
                //go.transform.SetAsFirstSibling();

                {
                    //go.EnableLayout();
                    go.transform.SetParent(mechLabPanel.rightArmWidget.transform, false);
                    go.GetComponent<LayoutElement>().ignoreLayout = true;
                    go.transform.GetChild("layout_armor").gameObject.SetActive(false);
                    go.transform.GetChild("layout_hardpoints").gameObject.SetActive(false);
                    go.transform.GetChild("layout_locationText").GetChild("txt_structure").gameObject.SetActive(false);
                    var rect = go.GetComponent<RectTransform>();
                    rect.pivot = new Vector2(0, 0);
                    rect.localPosition = new Vector3(0, 20);
                }

                go.name = "MechPropertiesWidget";
                go.transform.GetChild("layout_locationText").GetChild("txt_location").GetComponent<TextMeshProUGUI>().text = "General";
                go.SetActive(MechLabSlotsFeature.settings.MechLabGeneralWidgetEnabled);
                PropertiesWidget = go.GetComponent<MechLabLocationWidget>();
                PropertiesWidget.Init(mechLabPanel);
                var layout = new WidgetLayout(PropertiesWidget);

                // doesnt work with highlight frame
                //layout.layout_slots_glg.padding.top = 8;
                //layout.layout_slottedComponents_vlg.padding.top = 8;

                // doesnt work at all
                //var emptySpace = new GameObject("Cool GameObject made from Code");
                //var emptySpaceRect = emptySpace.AddComponent<RectTransform>();
                //emptySpaceRect.sizeDelta = new Vector2(1, 8);
                //emptySpace.AddComponent<LayoutElement>();
                //emptySpace.transform.SetParent(layout.layout_slots, false);
                //emptySpace.transform.SetAsFirstSibling();
                //emptySpace.SetActive(true);

                MechLabSlotsFixer.ModifyLayoutSlotCount(layout, MechLabSlotsFeature.settings.MechLabGeneralSlots);
            }

            {
                var mechRectTransform = mechLabPanel.leftArmWidget.transform.parent.parent.GetComponent<RectTransform>();
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
                Control.mod.Logger.LogError(e);
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