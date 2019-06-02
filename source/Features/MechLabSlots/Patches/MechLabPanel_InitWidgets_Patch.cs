using System;
using BattleTech.UI;
using Harmony;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.MechLabSlots.Patches
{
    [HarmonyPatch(typeof(MechLabPanel), "InitWidgets")]
    public static class MechLabPanel_InitWidgets_Patch
    {
        internal static MechLabLocationWidget MechPropertiesWidget;

        public static void Postfix(MechLabPanel __instance)
        {
            if (MechPropertiesWidget != null)
            {
                return;
            }

            try
            {
                var mechLabPanel = __instance;
                var Representation = mechLabPanel.transform.GetChild("Representation");
                var OBJ_mech = Representation.GetChild("OBJ_mech");
                
                foreach (Transform container in OBJ_mech)
                {
                    {
                        var go = container.gameObject;
                        go.EnableLayout();
                        var component = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
                        component.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                        component.enabled = true;
                    }

                    foreach (Transform widget in container)
                    {
                        if (widget.GetComponent<MechLabLocationWidget>() == null)
                        {
                            continue;
                        }

                        widget.gameObject.EnableLayout();
                        widget.GetChild("layout_slots").gameObject.EnableLayout();
                    }
                }

                {
                    var go = OBJ_mech.gameObject;
                    go.EnableLayout();
                    var component = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
                    component.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    component.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    component.enabled = true;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            try
            {
                {
                    var template = __instance.centerTorsoWidget;
                    var container = __instance.rightArmWidget.transform.parent.gameObject;
                    var clg = container.GetComponent<VerticalLayoutGroup>();
                    //clg.spacing = 20;
                    clg.padding = new RectOffset(0, 0, MechLabSlotsFeature.settings.MechLabArmTopPadding, 0);
                    var go = UnityEngine.Object.Instantiate(template.gameObject, null);
                    //go.transform.SetAsFirstSibling();

                    {
                        //go.EnableLayout();
                        go.transform.SetParent(__instance.rightArmWidget.transform, false);
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
                    MechPropertiesWidget = go.GetComponent<MechLabLocationWidget>();
                    MechPropertiesWidget.Init(__instance);
                    var layout = new MechLabLocationWidget_SetData_Patch.WidgetLayout(MechPropertiesWidget);
                    
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

                    MechLabLocationWidget_SetData_Patch.ModifySlotCount(layout, 3);
                }

                {
                    var mechRectTransform = __instance.leftArmWidget.transform.parent.parent.GetComponent<RectTransform>();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(mechRectTransform);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            try
            {
                var mechLabPanel = __instance;

                var Representation = mechLabPanel.transform.GetChild("Representation");
                var OBJ_mech = Representation.GetChild("OBJ_mech");

                var layout_details = OBJ_mech
                    .GetChild("Centerline")
                    .GetChild("layout_details");

                if (layout_details != null)
                {
                    var go = layout_details.gameObject;
                    go.EnableLayout();
                    go.GetComponent<LayoutElement>().ignoreLayout = true;

                    var leftArm = OBJ_mech.GetChild("LeftArm");
                    var vlg = leftArm.GetComponent<VerticalLayoutGroup>();
                    vlg.padding = new RectOffset(0, 0, MechLabSlotsFeature.settings.MechLabArmTopPadding, 0);
                    //layout_details.parent = leftArm;
                    //layout_details.SetAsFirstSibling();

                    var leftArmWidget = leftArm.GetChild(0);
                    layout_details.SetParent(leftArmWidget, false);
                    var rect = go.GetComponent<RectTransform>();
                    rect.pivot = new Vector2(0, 0);
                    rect.localPosition = new Vector3(0, 0);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}