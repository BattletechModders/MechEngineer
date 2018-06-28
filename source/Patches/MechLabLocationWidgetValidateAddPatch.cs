﻿using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabLocationWidget), "ValidateAdd", new[] { typeof(MechComponentDef) })]
    public static class MechLabLocationWidgetValidateAddPatch
    {
        public static void Postfix(
            MechLabLocationWidget __instance,
            MechComponentDef newComponentDef,
            List<MechLabItemSlotElement> ___localInventory,
            ref string ___dropErrorMessage,
            ref bool __result)
        {
            try
            {
                if (!__result)
                {
                    return;
                }

                //Gyro.ValidateAdd(newComponentDef, ___localInventory, ref ___dropErrorMessage, ref __result);
                //if (!__result)
                //{
                //    return;
                //}

                LegUpgrade.ValidateAdd(newComponentDef, ___localInventory, ref ___dropErrorMessage, ref __result);
                if (!__result)
                {
                    return;
                }

                //EngineMisc.EngineSlotsValidateAdd(newComponentDef, ___localInventory, ref ___dropErrorMessage, ref __result);
                //if (!__result)
                //{
                //    return;
                //}

                //EngineMisc.EngineCoreValidateAdd(newComponentDef, ___localInventory, ref ___dropErrorMessage, ref __result);
                //if (!__result)
                //{
                //    return;
                //}

                //Cockpit.ValidateAdd(newComponentDef, ___localInventory, ref ___dropErrorMessage, ref __result);
                //if (!__result)
                //{
                //    return;
                //}
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}