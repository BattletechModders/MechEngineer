using System;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(SimGameState), "CreateComponentInstallWorkOrder")]
    public static class EngineSimGameStateWorkOrderPatch
    {
        // change engine installation costs
        public static void Postfix(SimGameState __instance, MechComponentRef mechComponent, ref WorkOrderEntry_InstallComponent __result)
        {
            try
            {
                if (mechComponent == null)
                {
                    return;
                }

                var engine = Engine.MainEngineFromDef(mechComponent.Def);
                if (engine == null)
                {
                    return;
                }

                __result.SetCost(Control.calc.CalcInstallTechCost(engine));
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}