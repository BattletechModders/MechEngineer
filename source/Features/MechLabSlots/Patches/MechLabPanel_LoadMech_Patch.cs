using System;
using System.Linq;
using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabPanel), "LoadMech")]
    public static class MechLabPanel_LoadMech_Patch
    {
        public static void Postfix(MechLabPanel __instance)
        {
            try
            {
                var mechLabPanel = __instance;
                //GUILogUtils.LogHierarchy(mechLabPanel.transform);

                //mechLabPanel.transform.localScale = new Vector3(0.5f, 0.5f);
                var Representation = mechLabPanel.transform.GetChild("Representation");
                var OBJ_mech = Representation.GetChild("OBJ_mech");

                {
                    var scale = Control.settings.MechLabScale;
                    if (scale.HasValue)
                    {
                        OBJ_mech.transform.localScale = new Vector3(scale.Value, scale.Value);
                    }

                    var postitionY = Control.settings.MechLabPositionY;
                    if (postitionY.HasValue)
                    {
                        var rect = OBJ_mech.GetComponent<RectTransform>();
                        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, postitionY.Value);
                    }
                }

                var Centerline = OBJ_mech.GetChild("Centerline");
                {
                    var layout_details = Centerline.GetChild("layout_details");
                    if (layout_details == null)
                    {
                        layout_details = Representation.GetChild("layout_details");
                    }

                    if (layout_details != null)
                    {
                        var OBJ_value = Representation.GetChild("OBJ_value");
                        var LeftArmWidget = OBJ_mech.GetChild("LeftArm").GetChild("uixPrfPanl_ML_location-Widget-MANAGED");

                        var v = new Vector3[4];
                        LeftArmWidget.Rect().GetWorldCorners(v);
                        var armtop = v[1].y;
                        var armleft = v[2].x;
                        var armright = v[3].x;
                        var armcenter_x = armleft + (armleft - armright) / 2;

                        layout_details.SetParent(Representation, true);
                        layout_details.Rect().pivot = new Vector2(1.0f, 1.0f);
                        layout_details.position = new Vector3(
                            armcenter_x,
                            armtop + layout_details.Rect().sizeDelta.y + 10,
                            OBJ_value.position.z
                        );
                    }
                }

                const float moveUp = 0;
                const float space = 50;
                {
                    var headWidget = Centerline.GetChild("uixPrfPanl_ML_location-Widget-MANAGED");
                    var centerTorsoWidget = Centerline.GetChild("uixPrfPanl_ML_location-Widget-MANAGED", 1);

                    headWidget.SetTop(headWidget.Top() + moveUp);
                    centerTorsoWidget.SetTop(headWidget.Bottom() - space);
                }

                {
                    var RightTorsoLeg = OBJ_mech.GetChild("RightTorsoLeg");
                    var RightTorsoWidget = RightTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED");
                    var RightLegWidget = RightTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED", 1);

                    RightTorsoWidget.SetTop(RightTorsoWidget.Top() + moveUp);
                    RightLegWidget.SetTop(RightTorsoWidget.Bottom() - space);
                }

                {
                    var LeftTorsoLeg = OBJ_mech.GetChild("LeftTorsoLeg");
                    var LeftTorsoWidget = LeftTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED");
                    var LeftLegWidget = LeftTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED", 1);
                    
                    LeftTorsoWidget.SetTop(LeftTorsoWidget.Top() + moveUp);
                    LeftLegWidget.SetTop(LeftTorsoWidget.Bottom() - space);
                }

                //if (Control.settings.MechLabPanelLocationRepairButtonsHidden)
                //{
                //    foreach (var button in mechLabPanel.transform.GetComponentsInChildren<Transform>()
                //        .Where(t => t.name == "uixPrfBttn_BASE_repairButton-MANAGED"))
                //    {
                //        button.gameObject.SetActive(false);
                //    }
                //}
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}