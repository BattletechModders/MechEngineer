using System;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    [CustomComponent("MechConfiguration")]
    public class MechConfiguration : SimpleCustomComponent
    {
        public static void SetParent(Transform @this, Transform parent, bool worldPositionStays)
        {
            try
            {
                var element = @this.GetComponent<MechLabItemSlotElement>();
                if (IsMechConfiguration(element.ComponentRef.Def))
                {
                    var inventoryParent = Traverse
                        .Create(MechLabPanel_InitWidgets_Patch.MechPropertiesWidget)
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

        public static bool IsMechConfiguration(MechComponentDef def)
        {
            return def.Is<MechConfiguration>();
            //return def.Is<Flags>(out var f) && f.IsSet("mech_configuration");
        }
    }
}