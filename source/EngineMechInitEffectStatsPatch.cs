using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(Mech), "InitEffectStats")]
    public static class EngineMechInitEffectStatsPatch
    {
        // change the movement stats when loading into a combat game the first time
        public static void Postfix(Mech __instance)
        {
            try
            {
                var engine = __instance.MechDef.Inventory
                    .Select(x => Engine.MainEngineFromDef(x.Def))
                    .FirstOrDefault(x => x != null);

                if (engine == null)
                {
                    __instance.StatCollection.GetStatistic("HeatSinkCapacity").SetValue(Control.settings.FallbackHeatSinkCapacity);
                    return;
                }
                    
                var tonnage = __instance.tonnage;

                float walkSpeed, runSpeed, TTWalkSpeed;
                Control.calc.CalcSpeeds(engine, tonnage, out walkSpeed, out runSpeed, out TTWalkSpeed);

                __instance.StatCollection.GetStatistic("WalkSpeed").SetValue(walkSpeed);
                __instance.StatCollection.GetStatistic("RunSpeed").SetValue(runSpeed);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}