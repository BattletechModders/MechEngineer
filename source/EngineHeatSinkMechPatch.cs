using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(Mech), "GetHeatSinkDissipation")]
    public static class EngineHeatSinkMechPatch
    {
        // get heat dissipation rate of the engine by inventory and rating
        public static void Postfix(Mech __instance, ref float __result)
        {
            try
            {
                var engine = __instance.MechDef.Inventory
                    .Select(x => Engine.MainEngineFromDef(x.Def))
                    .FirstOrDefault(x => x != null);

                if (engine == null)
                {
                    __result += Control.settings.FallbackHeatSinkCapacity;
                    return;
                }

                var heatSink = __instance.MechDef.Inventory
                    .Where(c => c.ComponentDefType == ComponentType.HeatSink)
                    .Select(c => c.Def as HeatSinkDef)
                    .Where(c => c != null)
                    .FirstOrDefault(cd => cd.IsDouble() || cd.IsSingle());

                if (heatSink == null)
                {
                    __result += Control.settings.FallbackHeatSinkCapacity;
                    return;
                }

                __result += Control.calc.CalcHeatSinks(engine) * heatSink.DissipationCapacity;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}