using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechLabInventoryWidget), "RefreshJumpJetOptions")]
    public static class EngineMechLabInventoryWidgetPatch
    {
        // hide incompatible engines
        public static void Postfix(MechLabInventoryWidget __instance)
        {
            try
            {
                var tonnage = Traverse.Create(__instance).Field("mechTonnage").GetValue<float>();
                if (tonnage <= 0)
                {
                    return;
                }

                foreach (var element in __instance.localInventory)
                {
                    MechComponentDef componentDef;
                    if (element.controller != null)
                    {
                        componentDef = element.controller.componentDef;
                    }
                    else if (element.ComponentRef != null)
                    {
                        componentDef = element.ComponentRef.Def;
                    }
                    else
                    {
                        continue;
                    }

                    var engine = Engine.MainEngineFromDef(componentDef);
                    if (engine == null)
                    {
                        continue;
                    }
                        
                    if (Control.calc.CalcAvailability(engine, tonnage))
                    {
                        continue;
                    }

                    element.gameObject.SetActive(false);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}