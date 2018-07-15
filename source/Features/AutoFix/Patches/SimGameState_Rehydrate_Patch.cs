using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(SimGameState), "Rehydrate")]
    public static class SimGameState_Rehydrate_Patch
    {
        public static void Postfix(StatCollection ___companyStats, Dictionary<int, MechDef> ___ActiveMechs, Dictionary<int, MechDef> ___ReadyingMechs)
        {
            try
            {
                var mechs = new List<MechDef>();
                mechs.AddRange(___ActiveMechs.Values);
                mechs.AddRange(___ReadyingMechs.Values);
                MechDefAutoFixFacade.Rehydrate(___companyStats, mechs);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}