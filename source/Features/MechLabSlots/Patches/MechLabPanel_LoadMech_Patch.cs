using System;
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
                //GUILogUtils.LogHierarchy(__instance.transform);

                var Representation = __instance.transform.GetChild("Representation");
                var OBJ_mech = Representation.GetChild("OBJ_mech");

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

                const float space = 20;

                {
                    var headWidget = Centerline.GetChild("uixPrfPanl_ML_location-Widget-MANAGED");
                    var centerTorsoWidget = Centerline.GetChild("uixPrfPanl_ML_location-Widget-MANAGED", 1);

                    centerTorsoWidget.SetTop(headWidget.Bottom() - space);
                }

                {
                    var RightTorsoLeg = OBJ_mech.GetChild("RightTorsoLeg");
                    var RightTorsoWidget = RightTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED");
                    var RightLegWidget = RightTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED", 1);

                    RightLegWidget.SetTop(RightTorsoWidget.Bottom() - space);
                }

                {
                    var LeftTorsoLeg = OBJ_mech.GetChild("LeftTorsoLeg");
                    var LeftTorsoWidget = LeftTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED");
                    var LeftLegWidget = LeftTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED", 1);

                    LeftLegWidget.SetTop(LeftTorsoWidget.Bottom() - space);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}